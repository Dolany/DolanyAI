namespace Dolany.Ai.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.ExceptionServices;
    using System.Threading.Tasks;

    using AITools;

    using Base;

    using Cache;

    using Common;

    using Dolany.Ai.Common;
    using Model;

    using SyntaxChecker;

    /// <summary>
    /// AI管理类
    /// </summary>
    public class AIMgr
    {
        private IList<KeyValuePair<AIBase, AIAttribute>> AIList { get; set; }

        public string[] ManulOpenAiNames { get; set; }

        public static AIMgr Instance { get; } = new AIMgr();

        private List<IAITool> Tools { get; } = new List<IAITool>();

        public List<EnterCommandAttribute> AllAvailableGroupCommands { get; } = new List<EnterCommandAttribute>();

        public List<ISyntaxChecker> Checkers { get; private set; } = new List<ISyntaxChecker>();

        private delegate void MessageCallBack(string msg);

        private event MessageCallBack OnMessageCallBack;

        private AIMgr()
        {
        }

        public void MessagePublish(string message)
        {
            OnMessageCallBack?.Invoke($"{DateTime.Now}: {message}");
        }

        public void Load(Action<string> CallBackFunc = null)
        {
            if (CallBackFunc != null)
            {
                OnMessageCallBack += new MessageCallBack(CallBackFunc);
            }

            try
            {
                Init();
            }
            catch (Exception ex)
            {
                RuntimeLogger.Log(ex);
            }

            RuntimeLogger.Log("start up");
            DbMgr.InitXmls();
            RuntimeLogger.Log("加载所有可用AI");

            StartAIs();

            Sys_StartTime.Set(DateTime.Now);
        }

        /// <summary>
        /// 加载AI
        /// </summary>
        private void StartAIs()
        {
            AIList = AIList.Where(a => a.Value.Enable).OrderByDescending(a => a.Value.PriorityLevel).ToList();
            var count = AIList.Count;

            for (var i = 0; i < AIList.Count; i++)
            {
                AIList[i].Key.Initialization();
                ExtractCommands(AIList[i].Key);

                RuntimeLogger.Log($"AI加载进度：{AIList[i].Value.Name}({i + 1}/{count})");
            }

            foreach (var tool in Tools)
            {
                tool.Work();
            }

            ManulOpenAiNames = AIList.Where(ai => ai.Value.NeedManulOpen).Select(ai => ai.Value.Name).ToArray();
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

            Waiter.Instance.Listen();
        }

        private void LoadCheckers()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var list = from type in assembly.GetTypes()
                       where typeof(ISyntaxChecker).IsAssignableFrom(type) && type.IsClass
                       where type.FullName != null
                       let checker = assembly.CreateInstance(type.FullName) as ISyntaxChecker
                       select checker;

            Checkers = list.ToList();
        }

        private void LoadTools()
        {
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsClass || !typeof(IAITool).IsAssignableFrom(type))
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

            RuntimeLogger.Log($"{Tools.Count} tools created.");
            RuntimeLogger.Log(Global.IsTesting ? "Mode:Testing" : "Mode:Formal");
        }

        private void LoadAis()
        {
            var assembly = Assembly.GetAssembly(typeof(AIBase));
            var list = from type in assembly.GetTypes()
                       where type.IsSubclassOf(typeof(AIBase))
                       where type.FullName != null
                       let ai = assembly.CreateInstance(type.FullName) as AIBase
                       let attr = type.GetCustomAttribute(typeof(AIAttribute), false) as AIAttribute
                       select new KeyValuePair<AIBase, AIAttribute>(ai, attr);

            AIList = list.ToList();
        }

        [HandleProcessCorruptedStateExceptions]
        public void OnMsgReceived(MsgInformation MsgDTO)
        {
            try
            {
                if (AIList.IsNullOrEmpty())
                {
                    return;
                }

                if (Global.IsTesting && !Global.TestGroups.Contains(MsgDTO.FromGroup))
                {
                    return;
                }

                var msgEx = new MsgInformationEx
                                {
                                    Id = MsgDTO.Id,
                                    Msg = MsgDTO.Msg,
                                    RelationId = MsgDTO.RelationId,
                                    Time = MsgDTO.Time,
                                    FromGroup = MsgDTO.FromGroup,
                                    FromQQ = MsgDTO.FromQQ
                                };
                if (msgEx.FromQQ < 0)
                {
                    msgEx.FromQQ = msgEx.FromQQ & 0xFFFFFFFF;
                }

                var msg = msgEx.Msg;
                msgEx.FullMsg = msg;
                msgEx.Command = GenCommand(ref msg);
                msgEx.Msg = msg;
                msgEx.Type = msgEx.FromGroup == 0 ? MsgType.Private : MsgType.Group;

                MsgCallBack(msgEx);
            }
            catch (Exception ex)
            {
                RuntimeLogger.Log(ex);
            }
        }

        [HandleProcessCorruptedStateExceptions]
        private void MsgCallBack(MsgInformationEx MsgDTO)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (DirtyFilter.Instance.IsInBlackList(MsgDTO.FromQQ) || !DirtyFilter.Instance.Filter(MsgDTO.FromGroup, MsgDTO.FromQQ, MsgDTO.Msg))
                    {
                        return;
                    }

                    if (!AIList.Any(ai => ai.Key.OnMsgReceived(MsgDTO)))
                    {
                        return;
                    }

                    RecentCommandCache.Cache();
                    Sys_CommandCount.Plus();
                }
                catch (Exception ex)
                {
                    RuntimeLogger.Log(ex);
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
            if (strs.Length == 0)
            {
                return string.Empty;
            }

            var command = strs[0];
            msg = msg.Substring(command.Length, msg.Length - command.Length).Trim();
            return command;
        }

        public void OnActiveStateChange(bool state, long GroupNum)
        {
            foreach (var ai in AIList)
            {
                ai.Key.OnActiveStateChange(state, GroupNum);
            }
        }
    }
}
