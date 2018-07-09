using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Flexlive.CQP.Framework.Utils;
using AILib.Entities;
using System.ComponentModel.Composition;

namespace AILib
{
    /// <summary>
    /// AI管理类
    /// </summary>
    public class AIMgr
    {
        // 当前加载的AI列表
        [ImportMany(typeof(AIBase))]
        public IEnumerable<Lazy<AIBase>> AIList;

        public MsgReceiveCache MsgReceiveCache;

        public DirtyFilter Filter;

        private static AIMgr _instance;

        // 所有可用的AI列表
        public List<AIInfoDTO> AllAIs
        {
            get
            {
                List<AIInfoDTO> list = new List<AIInfoDTO>();
                Assembly assembly = Assembly.GetExecutingAssembly();
                Type[] typeArr = assembly.GetTypes();

                foreach (Type t in typeArr)
                {
                    object[] attributes = t.GetCustomAttributes(typeof(AIAttribute), false);
                    if (attributes.Length <= 0 || !(attributes[0] is AIAttribute))
                    {
                        continue;
                    }
                    AIAttribute attr = attributes[0] as AIAttribute;
                    if (!attr.IsAvailable)
                    {
                        continue;
                    }
                    list.Add(new AIInfoDTO()
                    {
                        Name = attr.Name,
                        Description = attr.Description
                    });
                }

                return list;
            }
        }

        public List<EnterCommandAttribute> AllAvailableCommands { get; private set; }

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
        /// 加载指定列表中的AI
        /// </summary>
        /// <param name="AINames">AI名称列表</param>
        /// <param name="ConfigDTO">AI配置DTO</param>
        public void StartAIs()
        {
            AIList = AIList.OrderByDescending(a => a.Value.PriorityLevel).ToList();
            foreach (var ai in AIList)
            {
                ai.Value.Work();

                LoadCommands(ai.Value);
            }
        }

        private void Init()
        {
            MsgReceiveCache = new MsgReceiveCache(GroupMsgCallBack);
            AllAvailableCommands = new List<EnterCommandAttribute>();
            Filter = new DirtyFilter();
        }

        private void LoadCommands(AIBase ai)
        {
            Type t = ai.GetType();
            foreach (var method in t.GetMethods())
            {
                foreach (var attr in method.GetCustomAttributes(typeof(EnterCommandAttribute), false))
                {
                    var enterAttr = attr as EnterCommandAttribute;
                    AllAvailableCommands.Add(enterAttr);
                }
            }
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

            if (Filter.IsInBlackList(MsgDTO.fromQQ) || !Filter.Filter(MsgDTO.fromQQ, MsgDTO.msg))
            {
                return;
            }

            string msg = MsgDTO.msg;
            MsgDTO.fullMsg = msg;
            MsgDTO.command = GenCommand(ref msg);
            MsgDTO.msg = msg;

            MsgReceiveCache.PushMsg(MsgDTO);
        }

        private void GroupMsgCallBack(GroupMsgDTO MsgDTO)
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
            var query = DbMgr.Query<AISealEntity>(s => s.GroupNum == MsgDTO.fromGroup && s.Content == ai.GetType().Name);
            return !query.IsNullOrEmpty();
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

            if (Filter.IsInBlackList(MsgDTO.fromQQ) || !Filter.Filter(MsgDTO.fromQQ, MsgDTO.msg))
            {
                return;
            }

            string msg = MsgDTO.msg;
            MsgDTO.command = GenCommand(ref msg);
            MsgDTO.msg = msg;

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