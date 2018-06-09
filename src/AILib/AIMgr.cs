using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AILib
{
    public static class AIMgr
    {
        // 当前加载的AI列表
        public static List<AIBase> AIList;

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

        /// <summary>
        /// 加载指定列表中的AI
        /// </summary>
        /// <param name="AINames">AI名称列表</param>
        /// <param name="ConfigDTO">AI配置DTO</param>
        public static void StartAIs( IEnumerable<string> AINames, AIConfigDTO ConfigDTO)
        {
            AIList = new List<AIBase>();

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
                if (!AINames.Contains(attr.Name))
                {
                    continue;
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
                if(ai != null)
                {
                    AIList.Add(ai);
                    ai.Work();
                }
            }
        }

        /// <summary>
        /// 处理群组消息收到事件
        /// </summary>
        /// <param name="MsgDTO"></param>
        public static void OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            try
            {
                if (AIList == null || AIList.Count == 0)
                {
                    return;
                }

                string msg = MsgDTO.msg;
                MsgDTO.command = GenCommand(ref msg);
                MsgDTO.msg = msg;

                foreach (var ai in AIList)
                {
                    ai.OnGroupMsgReceived(MsgDTO);
                }
            }
            catch(Exception ex)
            {
                Common.SendMsgToDeveloper(ex);
            }
        }

        /// <summary>
        /// 处理私聊消息收到事件
        /// </summary>
        /// <param name="MsgDTO"></param>
        public static void OnPrivateMsgReceived(PrivateMsgDTO MsgDTO)
        {
            if (AIList == null || AIList.Count == 0)
            {
                return;
            }

            string msg = MsgDTO.msg;
            MsgDTO.command = GenCommand(ref msg);
            MsgDTO.msg = msg;

            foreach (var ai in AIList)
            {
                if(ai.IsPrivateDeveloperOnly() && MsgDTO.fromQQ != Common.DeveloperNumber)
                {
                    continue;
                }
                ai.OnPrivateMsgReceived(MsgDTO);
            }
        }

        private static string GenCommand(ref string msg)
        {
            if(string.IsNullOrEmpty(msg))
            {
                return string.Empty;
            }

            string[] strs = msg.Split(new char[] { ' ' });
            if(strs == null || strs.Length == 0)
            {
                return string.Empty;
            }

            string command = strs[0];
            msg = msg.Substring(command.Length, msg.Length - command.Length);
            return command;
        }

        [AIDebug(EntrancePoint = "AIMgr_WorkingAIList")]
        public static string Debug_WorkingAIList
        {
            get
            {
                string result = $@"当前加载ai {AIList.Count}个";
                foreach(var ai in AIList)
                {
                    result += '\n' + $@"{ai.GetType().FullName}";
                }

                return result;
            }
        }

        public static bool DebugAIs(string EntrancePoint)
        {
            foreach(var ai in AIList)
            {
                foreach (var property in ai.GetType().GetProperties())
                {
                    foreach (var attr in property.GetCustomAttributes(false))
                    {
                        if (!(attr is AIDebugAttribute))
                        {
                            continue;
                        }

                        AIDebugAttribute debugAttr = attr as AIDebugAttribute;
                        if (debugAttr.EntrancePoint != EntrancePoint)
                        {
                            continue;
                        }

                        object obj = ai.GetType().GetProperty(property.Name).GetValue(ai);

                        Common.SendMsgToDeveloper(obj as string);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
