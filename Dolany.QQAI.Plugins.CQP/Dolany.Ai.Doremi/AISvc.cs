using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Doremi.AITools;
using Dolany.Ai.Doremi.Base;
using Dolany.Ai.Doremi.Cache;
using Dolany.Ai.Doremi.Common;
using Dolany.Ai.Doremi.SyntaxChecker;
using Dolany.Ai.Doremi.Xiuxian;
using Dolany.Database.Sqlite;

namespace Dolany.Ai.Doremi
{
    /// <summary>
    /// AI管理类
    /// </summary>
    public class AISvc
    {
        private List<AIBase> AIList { get; set; }

        public string[] ManulOpenAiNames { get; set; }

        private List<IAITool> Tools { get; } = new List<IAITool>();

        public List<EnterCommandAttribute> AllAvailableGroupCommands { get; } = new List<EnterCommandAttribute>();

        public List<ISyntaxChecker> Checkers { get; private set; } = new List<ISyntaxChecker>();

        public List<string> OptionalAINames => AIList.Where(ai => ai.AIAttr.NeedManulOpen).Select(ai => ai.AIAttr.Name).ToList();

        private delegate void MessageCallBack(string msg);

        private event MessageCallBack OnMessageCallBack;

        public RandShopperSvc RandShopperSvc { get; set; }
        public WaiterSvc WaiterSvc { get; set; }
        public PowerStateSvc PowerStateSvc { get; set; }
        public DirtyFilterSvc DirtyFilterSvc { get; set; }

        public T AIInstance<T>() where T : AIBase
        {
            return AIList.FirstOrDefault(ai => ai.GetType().Name == typeof(T).Name) as T;
        }

        public void MessagePublish(string message)
        {
            Console.Title = "Server_Doremi";
            OnMessageCallBack?.Invoke($"{DateTime.Now}: {message}");
        }

        public void Load(Action<string> CallBackFunc = null)
        {
            Logger.Log("start up");
            if (CallBackFunc != null)
            {
                OnMessageCallBack += new MessageCallBack(CallBackFunc);
            }

            try
            {
                Init();
                Logger.Log("加载所有可用AI");
                StartAIs();
                WaiterSvc.Listen();
                RandShopperSvc.BindAi = "DoreFun";

                AIAnalyzer.Sys_StartTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        /// <summary>
        /// 加载AI
        /// </summary>
        private void StartAIs()
        {
            AIList = AIList.Where(a => a != null && a.AIAttr.Enable).OrderByDescending(a => a.AIAttr.PriorityLevel).ToList();
            var count = AIList.Count;

            for (var i = 0; i < AIList.Count; i++)
            {
                AIList[i].Initialization();
                ExtractCommands(AIList[i]);

                Logger.Log($"AI加载进度：{AIList[i].AIAttr.Name}({i + 1}/{count})");
            }

            foreach (var tool in Tools)
            {
                tool.Work();
            }

            ManulOpenAiNames = AIList.Where(ai => ai.AIAttr.NeedManulOpen).Select(ai => ai.AIAttr.Name).ToArray();
        }

        private void ExtractCommands(AIBase ai)
        {
            var type = ai.GetType();
            foreach (var method in type.GetMethods())
            {
                foreach (EnterCommandAttribute attr in method.GetCustomAttributes(typeof(EnterCommandAttribute), false))
                {
                    foreach (var command in attr.CommandsList)
                    {
                        var attrClone = attr.Clone();
                        attrClone.Command = command;
                        AllAvailableGroupCommands.Add(attrClone);
                    }
                }
            }
        }

        private void Init()
        {
            LoadAis();
            LoadTools();
            LoadCheckers();

            SFixedSetService.SetMaxCount("PicCache", Configger<AIConfigBase>.Instance.AIConfig.MaxPicCacheCount);
        }

        private void LoadCheckers()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var list = assembly.GetTypes()
                .Where(type => typeof(ISyntaxChecker).IsAssignableFrom(type) && type.IsClass)
                .Where(type => type.FullName != null)
                .Select(type => new {type, checker = assembly.CreateInstance(type.FullName) as ISyntaxChecker})
                .Select(t => t.checker);

            Checkers = list.ToList();
        }

        private void LoadTools()
        {
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsClass || !typeof(IAITool).IsAssignableFrom(type) || type.IsAbstract)
                {
                    continue;
                }

                if (type.FullName == null)
                {
                    continue;
                }

                if (assembly.CreateInstance(type.FullName) is IAITool tool && tool.Enable)
                {
                    Tools.Add(tool);
                }
            }

            Logger.Log($"{Tools.Count} tools created.");
            Logger.Log(Global.IsTesting ? "Mode:Testing" : "Mode:Formal");
        }

        private void LoadAis()
        {
            var assembly = Assembly.GetAssembly(typeof(AIBase));
            var list = assembly.GetTypes()
                .Where(type => type.IsSubclassOf(typeof(AIBase)))
                .Where(type => type.FullName != null)
                .Select(type => assembly.CreateInstance(type.FullName) as AIBase);

            AIList = list.ToList();
        }

        [HandleProcessCorruptedStateExceptions]
        public void OnMsgReceived(MsgInformation MsgDTO)
        {
            // 群聊消息
            if (MsgDTO.FromGroup != 0)
            {
                if (!Global.AllGroupsDic.ContainsKey(MsgDTO.FromGroup))
                {
                    return;
                }

                if (Global.IsTesting && !Global.TestGroups.Contains(MsgDTO.FromGroup))
                {
                    return;
                }
            }

            var msgEx = new MsgInformationEx
            {
                Id = MsgDTO.Id,
                Msg = MsgDTO.Msg,
                RelationId = MsgDTO.RelationId,
                Time = MsgDTO.Time,
                FromGroup = MsgDTO.FromGroup,
                FromQQ = MsgDTO.FromQQ,
                BindAi = MsgDTO.BindAi
            };
            if (msgEx.FromQQ < 0)
            {
                msgEx.FromQQ &= 0xFFFFFFFF;
            }

            var msg = msgEx.Msg;
            msgEx.FullMsg = msg;
            msgEx.Command = GenCommand(ref msg);
            msgEx.Msg = msg;
            msgEx.Type = msgEx.FromGroup == 0 ? MsgType.Private : MsgType.Group;

            MsgCallBack(msgEx);
        }

        [HandleProcessCorruptedStateExceptions]
        private void MsgCallBack(MsgInformationEx MsgDTO)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (!DirtyFilterSvc.Filter(MsgDTO))
                    {
                        return;
                    }

                    foreach (var ai in AIList)
                    {
                        MsgDTO.BindAi = ai.AIAttr.BindAi;
                        if (ai.AIAttr.PriorityLevel < 100 && !PowerStateSvc.CheckPower(MsgDTO.BindAi))
                        {
                            continue;
                        }

                        if (RecentCommandCache.IsTooFreq(MsgDTO.BindAi))
                        {
                            continue;
                        }

                        if (ai.OnMsgReceived(MsgDTO))
                        {
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            });
        }

        private static string GenCommand(ref string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return string.Empty;
            }

            var strs = msg.Split(' ');
            if (strs.IsNullOrEmpty())
            {
                return string.Empty;
            }

            var command = strs[0];
            msg = msg.Substring(command.Length, msg.Length - command.Length).Trim();
            return command;
        }
    }
}
