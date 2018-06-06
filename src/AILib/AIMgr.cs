﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AILib
{
    public static class AIMgr
    {
        public static List<AIBase> AIList;

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

        public static void StartAIs(
            IEnumerable<string> AINames,
            AIConfigDTO ConfigDTO)
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

                AIBase ai = 
                assembly.CreateInstance(
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

        public static void OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            if(AIList == null || AIList.Count == 0)
            {
                return;
            }

            foreach(var ai in AIList)
            {
                ai.OnGroupMsgReceived(MsgDTO);
            }
        }

        public static void OnPrivateMsgReceived(PrivateMsgDTO MsgDTO)
        {
            if (AIList == null || AIList.Count == 0)
            {
                return;
            }

            foreach (var ai in AIList)
            {
                ai.OnPrivateMsgReceived(MsgDTO);
            }
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
