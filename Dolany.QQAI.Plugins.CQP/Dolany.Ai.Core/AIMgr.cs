namespace Dolany.Ai.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.ExceptionServices;
    using System.Threading.Tasks;

    using Dolany.Ai.Core.AITools;
    using Dolany.Ai.Core.Base;
    using Dolany.Ai.Core.Cache;
    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Model;
    using Dolany.Ai.Core.SyntaxChecker;
    using Dolany.Database.Ai;

    /// <summary>
    /// AI管理类
    /// </summary>
    public class AIMgr
    {
        public IEnumerable<KeyValuePair<AIBase, AIAttribute>> AIList { get; private set; }

        public static AIMgr Instance { get; } = new AIMgr();

        private List<IAITool> Tools { get; } = new List<IAITool>();

        public List<EnterCommandAttribute> AllAvailableGroupCommands { get; } = new List<EnterCommandAttribute>();

        public List<ISyntaxChecker> Checkers { get; set; } = new List<ISyntaxChecker>();

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

            var msg = $"成功加载{AIList.Count()}个ai \r\n";
            RuntimeLogger.Log(msg);

            Sys_StartTime.Set(DateTime.Now);
        }

        /// <summary>
        /// 加载AI
        /// </summary>
        private void StartAIs()
        {
            AIList = AIList.Where(a => a.Value.IsAvailable)
                           .OrderByDescending(a => a.Value.PriorityLevel);
            foreach (var ai in AIList)
            {
                ai.Key.Work();
                ExtractCommands(ai.Key);
            }

            foreach (var tool in Tools)
            {
                tool.Work();
            }
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
                if (!type.IsClass ||
                    !typeof(IAITool).IsAssignableFrom(type))
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

                var MsgEx = new MsgInformationEx
                                {
                                    Id = MsgDTO.Id,
                                    Msg = MsgDTO.Msg,
                                    RelationId = MsgDTO.RelationId,
                                    Time = MsgDTO.Time,
                                    FromGroup = MsgDTO.FromGroup,
                                    FromQQ = MsgDTO.FromQQ
                                };
                if (MsgEx.FromQQ < 0)
                {
                    MsgEx.FromQQ = MsgEx.FromQQ & 0xFFFFFFFF;
                }

                var msg = MsgEx.Msg;
                MsgEx.FullMsg = msg;
                MsgEx.Command = GenCommand(ref msg);
                MsgEx.Msg = msg;
                MsgEx.Type = MsgEx.FromGroup == 0 ? MsgType.Private : MsgType.Group;

                MsgCallBack(MsgEx);
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
                    MsgCallBack_Func(MsgDTO);
                }
                catch (Exception ex)
                {
                    RuntimeLogger.Log(ex);
                }
            });
        }

        private void MsgCallBack_Func(MsgInformationEx MsgDTO)
        {
            if (DirtyFilter.IsInBlackList(MsgDTO.FromQQ) ||
                !DirtyFilter.Filter(MsgDTO.FromGroup, MsgDTO.FromQQ, MsgDTO.Msg))
            {
                return;
            }

            if (MsgDTO.Type == MsgType.Group)
            {
                MsgDTO.AuthName = Utility.GetAuthName(MsgDTO);
            }

            if (!AIList.Where(ai => !IsAiSealed(MsgDTO, ai.Key))
                .Any(ai => ai.Key.OnMsgReceived(MsgDTO)))
            {
                return;
            }

            RecentCommandCache.Cache(DateTime.Now);
            Sys_CommandCount.Plus();
        }

        private static bool IsAiSealed(MsgInformation MsgDTO, AIBase ai)
        {
            using (var db = new AIDatabase())
            {
                var aiName = ai.GetType().Name;
                var query = db.AISeal.Where(s => s.GroupNum == MsgDTO.FromGroup &&
                                                 s.AiName == aiName);
                return !query.IsNullOrEmpty();
            }
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
