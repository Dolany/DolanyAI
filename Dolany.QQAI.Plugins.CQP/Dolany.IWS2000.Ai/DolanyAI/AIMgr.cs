using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

// ReSharper disable UnusedParameter.Local

namespace Dolany.IWS2000.Ai.DolanyAI
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

        // ReSharper disable once CollectionNeverQueried.Global
        public List<GroupEnterCommandAttribute> AllAvailableGroupCommands { get; } = new List<GroupEnterCommandAttribute>();

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
                foreach (GroupEnterCommandAttribute attr in method.GetCustomAttributes(typeof(GroupEnterCommandAttribute), false))
                {
                    AllAvailableGroupCommands.Add(attr);
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
            var list = (from type in assembly.GetTypes()
                        where type.IsSubclassOf(typeof(AIBase))
                        where type.FullName != null
                        let ai = assembly.CreateInstance(type.FullName) as AIBase
                        let attr = type.GetCustomAttribute(typeof(AIAttribute), false) as AIAttribute
                        select new KeyValuePair<AIBase, AIAttribute>(ai, attr)
                        ).ToList();

            AIList = list;
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

                if (type.FullName == null) continue;
                var tool = assembly.CreateInstance(type.FullName) as IAITool;

                Tools.Add(tool);
            }

            RuntimeLogger.Log($"{Tools.Count} tools created.");
        }

        /// <summary>
        /// 处理群组消息收到事件
        /// </summary>
        /// <param name="MsgDTO"></param>
        public void OnGroupMsgReceived(GroupMsgDTO MsgDTO)
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

            GroupMsgCallBack(MsgDTO);
        }

        private void GroupMsgCallBack(GroupMsgDTO MsgDTO)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    GroupMsgCallBack_Func(MsgDTO);
                }
                catch (Exception ex)
                {
                    RuntimeLogger.Log(ex);
                }
            });
        }

        private void GroupMsgCallBack_Func(GroupMsgDTO MsgDTO)
        {
            foreach (var ai in AIList)
            {
                if (ai.Key.OnGroupMsgReceived(MsgDTO))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 处理私聊消息收到事件
        /// </summary>
        /// <param name="MsgDTO"></param>
        public void OnPrivateMsgReceived(PrivateMsgDTO MsgDTO)
        {
            if (AIList.IsNullOrEmpty())
            {
                return;
            }

            var msg = MsgDTO.Msg;
            MsgDTO.Command = GenCommand(ref msg);
            MsgDTO.Msg = msg;

            foreach (var ai in AIList)
            {
                ai.Key.OnPrivateMsgReceived(MsgDTO);
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