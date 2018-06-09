﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AILib
{
    public abstract class AIBase
    {
        public AIConfigDTO ConfigDTO { get; set; }

        public abstract void Work();

        public AIBase(AIConfigDTO ConfigDTO)
        {
            this.ConfigDTO = ConfigDTO;
        }

        public virtual void OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            Type t = this.GetType();
            foreach(var method in t.GetMethods())
            {
                foreach(var attr in method.GetCustomAttributes(typeof(EnterCommandAttribute), false))
                {
                    var enterAttr = attr as EnterCommandAttribute;
                    if (enterAttr.Command != MsgDTO.command || enterAttr.SourceType != MsgType.Group)
                    {
                        break;
                    }
                    t.InvokeMember(method.Name,
                            BindingFlags.InvokeMethod,
                            null,
                            this,
                            new object[] { MsgDTO }
                            );
                    break;
                }
            }
        }

        public virtual void OnPrivateMsgReceived(PrivateMsgDTO MsgDTO)
        {
            Type t = this.GetType();
            foreach (var method in t.GetMethods())
            {
                foreach (var attr in method.GetCustomAttributes(typeof(EnterCommandAttribute), false))
                {
                    var enterAttr = attr as EnterCommandAttribute;
                    if (enterAttr.Command != MsgDTO.command || enterAttr.SourceType != MsgType.Private)
                    {
                        break;
                    }
                    t.InvokeMember(method.Name,
                            BindingFlags.InvokeMethod,
                            null,
                            this,
                            new object[] { MsgDTO }
                            );
                    break;
                }
            }
        }

        public virtual bool IsPrivateDeveloperOnly()
        {
            return false;
        }
    }
}
