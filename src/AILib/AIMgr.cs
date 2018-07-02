using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Flexlive.CQP.Framework.Utils;
using AILib.Entities;

namespace AILib
{
    /// <summary>
    /// AI管理类
    /// </summary>
    public static class AIMgr
    {
        // 当前加载的AI列表
        public static List<AIBase> AIList;

        public static MsgReceiveCache MsgReceiveCache;

        // 所有可用的AI列表
        public static List<AIInfoDTO> AllAIs
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

        public static List<EnterCommandAttribute> AllAvailableCommands { get; private set; }

        /// <summary>
        /// 加载指定列表中的AI
        /// </summary>
        /// <param name="AINames">AI名称列表</param>
        /// <param name="ConfigDTO">AI配置DTO</param>
        public static void StartAIs(IEnumerable<string> AINames, AIConfigDTO ConfigDTO)
        {
            Init();

            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] typeArr = assembly.GetTypes();

            foreach (Type t in typeArr)
            {
                CreateAI(AINames, ConfigDTO, t, assembly);
            }

            AIList = AIList.OrderByDescending(a => a.PriorityLevel).ToList();
            foreach (var ai in AIList)
            {
                ai.Work();

                LoadCommands(ai);
            }
        }

        private static void Init()
        {
            AIList = new List<AIBase>();
            MsgReceiveCache = new MsgReceiveCache(GroupMsgCallBack);
            AllAvailableCommands = new List<EnterCommandAttribute>();
        }

        private static void CreateAI(IEnumerable<string> AINames, AIConfigDTO ConfigDTO, Type t, Assembly assembly)
        {
            object[] attributes = t.GetCustomAttributes(typeof(AIAttribute), false);
            if (attributes.Length <= 0 || !(attributes[0] is AIAttribute))
            {
                return;
            }
            AIAttribute attr = attributes[0] as AIAttribute;
            if (!AINames.Contains(attr.Name))
            {
                return;
            }

            AIBase ai = assembly.CreateInstance(
                t.FullName,
                true,
                BindingFlags.Default,
                null,
                new object[] { ConfigDTO },
                null,
                null
                ) as AIBase;
            if (ai != null)
            {
                ai.PriorityLevel = attr.PriorityLevel;
                AIList.Add(ai);
            }
        }

        private static void LoadCommands(AIBase ai)
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
        public static void OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            if (AIList.IsNullOrEmpty())
            {
                return;
            }

            string msg = MsgDTO.msg;
            MsgDTO.fullMsg = msg;
            MsgDTO.command = GenCommand(ref msg);
            MsgDTO.msg = msg;

            MsgReceiveCache.PushMsg(MsgDTO);
        }

        private static void GroupMsgCallBack(GroupMsgDTO MsgDTO)
        {
            try
            {
                foreach (var ai in AIList)
                {
                    if (IsAiSealed(MsgDTO, ai))
                    {
                        continue;
                    }

                    if (ai.OnGroupMsgReceived(MsgDTO))
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

        private static bool IsAiSealed(GroupMsgDTO MsgDTO, AIBase ai)
        {
            var query = DbMgr.Query<AISealEntity>(s => s.GroupNum == MsgDTO.fromGroup && s.Content == ai.GetType().Name);
            return !query.IsNullOrEmpty();
        }

        /// <summary>
        /// 处理私聊消息收到事件
        /// </summary>
        /// <param name="MsgDTO"></param>
        public static void OnPrivateMsgReceived(PrivateMsgDTO MsgDTO)
        {
            if (AIList.IsNullOrEmpty())
            {
                return;
            }

            string msg = MsgDTO.msg;
            MsgDTO.command = GenCommand(ref msg);
            MsgDTO.msg = msg;

            foreach (var ai in AIList)
            {
                ai.OnPrivateMsgReceived(MsgDTO);
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