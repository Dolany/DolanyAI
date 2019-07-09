using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.AITools;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.SyntaxChecker;
using Dolany.Database.Sqlite;

namespace Dolany.Ai.Core
{
    /// <summary>
    /// AI管理类
    /// </summary>
    public class AIMgr
    {
        private List<AIBase> AIList { get; set; }

        public string[] ManulOpenAiNames { get; set; }

        public static AIMgr Instance { get; } = new AIMgr();

        private List<IAITool> Tools { get; } = new List<IAITool>();

        public List<EnterCommandAttribute> AllAvailableGroupCommands { get; } = new List<EnterCommandAttribute>();

        public List<ISyntaxChecker> Checkers { get; private set; } = new List<ISyntaxChecker>();

        public List<string> OptionalAINames => AIList.Where(ai => ai.NeedManualOpeon).Select(ai => ai.AIName).ToList();

        private delegate void MessageCallBack(string msg);

        private event MessageCallBack OnMessageCallBack;

        public T AIInstance<T>() where T : AIBase
        {
            return AIList.FirstOrDefault(ai => ai.GetType().Name == typeof(T).Name) as T;
        }

        private AIMgr()
        {
        }

        public void MessagePublish(string message)
        {
            Console.Title = "Server";
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
                Waiter.Instance.Listen();

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
            AIList = AIList.Where(a => a != null && a.Enable).OrderByDescending(a => a.PriorityLevel).ToList();
            var count = AIList.Count;

            for (var i = 0; i < AIList.Count; i++)
            {
                AIList[i].Initialization();
                ExtractCommands(AIList[i]);

                Logger.Log($"AI加载进度：{AIList[i].AIName}({i + 1}/{count})");
            }

            foreach (var tool in Tools)
            {
                tool.Work();
            }

            ManulOpenAiNames = AIList.Where(ai => ai.NeedManualOpeon).Select(ai => ai.AIName).ToArray();
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

            SFixedSetService.SetMaxCount("PicCache", Global.DefaultConfig.MaxPicCacheCount);
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

                var tool = assembly.CreateInstance(type.FullName) as IAITool;
                Tools.Add(tool);
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
            try
            {
                if (!DirtyFilter.Instance.Filter(MsgDTO))
                {
                    return;
                }

                var availableBindAis = MsgDTO.Type == MsgType.Group
                    ? GroupSettingMgr.Instance[MsgDTO.FromGroup].BindAis.Where(p => !RecentCommandCache.IsTooFreq(p)).Select(p => BindAiMgr.Instance[p]).ToList()
                    : new List<BindAiModel>();
                foreach (var ai in AIList)
                {
                    if (MsgDTO.Type == MsgType.Group)
                    {
                        if (ai.IsAdvanced)
                        {
                            availableBindAis = availableBindAis.Where(p => p.IsAdvanced).ToList();
                        }

                        if (!availableBindAis.Any() && ai.PriorityLevel < 50)
                        {
                            continue;
                        }

                        MsgDTO.BindAi = availableBindAis.RandElement().Name;
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
