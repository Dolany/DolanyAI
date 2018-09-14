using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Dolany.Ice.Ai.DolanyAI.Db;

namespace Dolany.Ice.Ai.DolanyAI
{
    /// <summary>
    /// AI管理类
    /// </summary>
    public class AIMgr
    {
        public IEnumerable<KeyValuePair<AIBase, AIAttribute>> AIList;

        private static AIMgr _instance;

        public static AIMgr Instance => _instance ?? (_instance = new AIMgr());

        private List<IAITool> Tools { get; } = new List<IAITool>();
        public List<EnterCommandAttribute> AllAvailableGroupCommands { get; } = new List<EnterCommandAttribute>();

        private AIMgr()
        {
            try
            {
                Init();
            }
            catch (Exception ex)
            {
                RuntimeLogger.Log(ex);
            }
        }

        /// <summary>
        /// 加载AI
        /// </summary>
        public void StartAIs()
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
                        attr.Command = command;
                        AllAvailableGroupCommands.Add(attr);
                    }
                }
            }
        }

        private void Init()
        {
            LoadAis();
            LoadTools();
        }

        private void LoadAis()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var list = from type in assembly.GetTypes()
                       where type.IsSubclassOf(typeof(AIBase))
                       where type.FullName != null
                       let ai = assembly.CreateInstance(type.FullName) as AIBase
                       let attr = type.GetCustomAttribute(typeof(AIAttribute), false) as AIAttribute
                       select new KeyValuePair<AIBase, AIAttribute>(ai, attr);

            AIList = list;
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

        /// <summary>
        /// 处理群组消息收到事件
        /// </summary>
        /// <param name="MsgDTO"></param>
        public void OnMsgReceived(ReceivedMsgDTO MsgDTO)
        {
            try
            {
                if (AIList.IsNullOrEmpty())
                {
                    return;
                }

                if (MsgDTO.FromQQ < 0)
                {
                    MsgDTO.FromQQ = MsgDTO.FromQQ & 0xFFFFFFFF;
                }

                var msg = MsgDTO.Msg;
                MsgDTO.FullMsg = msg;
                MsgDTO.Command = GenCommand(ref msg);
                MsgDTO.Msg = msg;

                MsgCallBack(MsgDTO);
            }
            catch (StackOverflowException ex)
            {
                RuntimeLogger.Log(ex);
            }
        }

        private void MsgCallBack(ReceivedMsgDTO MsgDTO)
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

        private void MsgCallBack_Func(ReceivedMsgDTO MsgDTO)
        {
            if (DirtyFilter.IsInBlackList(MsgDTO.FromQQ) ||
                !DirtyFilter.Filter(MsgDTO.FromGroup, MsgDTO.FromQQ, MsgDTO.Msg))
            {
                return;
            }

            foreach (var ai in AIList)
            {
                if (IsAiSealed(MsgDTO, ai.Key))
                {
                    continue;
                }

                if (ai.Key.OnMsgReceived(MsgDTO))
                {
                    break;
                }
            }
        }

        private static bool IsAiSealed(ReceivedMsgDTO MsgDTO, AIBase ai)
        {
            using (var db = new AIDatabase())
            {
                var aiName = ai.GetType().Name;
                var query = db.AISeal.Where(s => s.GroupNum == MsgDTO.FromGroup &&
                                                 s.AiName == aiName);
                return !query.IsNullOrEmpty();
            }
        }

        /// <summary>
        /// 提取消息命令，并将消息修改为没有命令的部分
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
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
    }
}