using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public MsgReceiveCache MsgReceiveCache;

        public DirtyFilter Filter;

        private static AIMgr _instance;

        public List<GroupEnterCommandAttribute> AllAvailableGroupCommands = new List<GroupEnterCommandAttribute>();

        public static AIMgr Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AIMgr();
                }

                return _instance;
            }
        }

        public AIMgr()
        {
            Init();
        }

        /// <summary>
        /// 加载AI
        /// </summary>
        public void StartAIs()
        {
            AIList = AIList.Where(a => a.Value.IsAvailable)
                           .OrderByDescending(a => a.Value.PriorityLevel)
                           .GroupBy(a => a.Value.Name)
                           .Select(g => g.First());
            foreach (var ai in AIList)
            {
                ai.Key.Work();
                ExtractCommands(ai.Key);
            }
        }

        private void ExtractCommands(AIBase ai)
        {
            Type type = ai.GetType();
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

            MsgReceiveCache = new MsgReceiveCache(GroupMsgCallBack);
            Filter = new DirtyFilter();
        }

        private void LoadAis()
        {
            var list = new List<KeyValuePair<AIBase, AIAttribute>>();

            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsSubclassOf(typeof(AIBase)))
                {
                    continue;
                }

                var ai = assembly.CreateInstance(type.FullName);
                var attr = type.GetCustomAttribute(typeof(AIAttribute), false) as AIAttribute;

                list.Add(new KeyValuePair<AIBase, AIAttribute>(ai as AIBase, attr));
            }

            AIList = list;
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

            string msg = MsgDTO.Msg;
            MsgDTO.FullMsg = msg;
            MsgDTO.Command = GenCommand(ref msg);
            MsgDTO.Msg = msg;

            //MsgReceiveCache.PushMsg(MsgDTO);
            //GroupMsgCallBack(MsgDTO);
            GroupMsgCallBack_Func(MsgDTO);
        }

        private void GroupMsgCallBack(GroupMsgDTO MsgDTO)
        {
            Task.Factory.StartNew(() =>
            {
                GroupMsgCallBack_Func(MsgDTO);
            });
        }

        private void GroupMsgCallBack_Func(GroupMsgDTO MsgDTO)
        {
            try
            {
                if (Filter.IsInBlackList(MsgDTO.FromQQ) || !Filter.Filter(MsgDTO.FromGroup, MsgDTO.FromQQ, MsgDTO.Msg))
                {
                    return;
                }

                foreach (var ai in AIList)
                {
                    if (IsAiSealed(MsgDTO, ai.Key))
                    {
                        continue;
                    }

                    if (ai.Key.OnGroupMsgReceived(MsgDTO))
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.SendMsgToDeveloper(ex);
            }
        }

        private bool IsAiSealed(GroupMsgDTO MsgDTO, AIBase ai)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var aiName = ai.GetType().Name;
                var query = db.AISeal.Where(s => s.GroupNum == MsgDTO.FromGroup && s.AiName == aiName);
                return !query.IsNullOrEmpty();
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

            //if (Filter.IsInBlackList(MsgDTO.FromQQ) || !Filter.Filter(MsgDTO.FromQQ, MsgDTO.Msg))
            //{
            //    return;
            //}

            string msg = MsgDTO.Msg;
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
        private string GenCommand(ref string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return string.Empty;
            }

            string[] strs = msg.Split(new char[] { ' ' });
            if (strs == null || strs.Length == 0)
            {
                return string.Empty;
            }

            string command = strs[0];
            msg = msg.Substring(command.Length, msg.Length - command.Length).Trim();
            return command;
        }
    }
}