using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Flexlive.CQP.Framework.Utils;
using AILib.Entities;
using System.ComponentModel.Composition;
using AILib.Db;

namespace AILib
{
    /// <summary>
    /// AI管理类
    /// </summary>
    public class AIMgr
    {
        [ImportMany(typeof(AIBase))]
        public IEnumerable<Lazy<AIBase, IAIExportCapabilities>> AIList;

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
            this.ComposePartsSelf();

            Init();
        }

        /// <summary>
        /// 加载AI
        /// </summary>
        public void StartAIs()
        {
            AIList = AIList.Where(a => a.Metadata.IsAvailable)
                           .OrderByDescending(a => a.Metadata.PriorityLevel)
                           .GroupBy(a => a.Metadata.Name)
                           .Select(g => g.First())
                           .ToList();
            foreach (var ai in AIList)
            {
                ai.Value.Work();
                ExtractCommands(ai.Value);
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
            MsgReceiveCache = new MsgReceiveCache(GroupMsgCallBack);
            Filter = new DirtyFilter();
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

            if (Filter.IsInBlackList(MsgDTO.FromQQ) || !Filter.Filter(MsgDTO.FromGroup, MsgDTO.FromQQ, MsgDTO.Msg))
            {
                return;
            }

            string msg = MsgDTO.Msg;
            MsgDTO.FullMsg = msg;
            MsgDTO.Command = GenCommand(ref msg);
            MsgDTO.Msg = msg;

            MsgReceiveCache.PushMsg(MsgDTO);
        }

        private void GroupMsgCallBack(GroupMsgDTO MsgDTO)
        {
            Task.Run(new Action(() =>
            {
                GroupMsgCallBack_Func(MsgDTO);
            }));
        }

        private void GroupMsgCallBack_Func(GroupMsgDTO MsgDTO)
        {
            try
            {
                foreach (var ai in AIList)
                {
                    if (IsAiSealed(MsgDTO, ai.Value))
                    {
                        continue;
                    }

                    if (ai.Value.OnGroupMsgReceived(MsgDTO))
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SendMsgToDeveloper(ex);
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
                ai.Value.OnPrivateMsgReceived(MsgDTO);
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