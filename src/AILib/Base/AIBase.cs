using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Flexlive.CQP.Framework;

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
            if (MsgDTO.fromQQ < 0)
            {
                MsgDTO.fromQQ = MsgDTO.fromQQ & 0xFFFFFFFF;
            }

            Type t = this.GetType();
            foreach(var method in t.GetMethods())
            {
                foreach(var attr in method.GetCustomAttributes(typeof(EnterCommandAttribute), false))
                {
                    var enterAttr = attr as EnterCommandAttribute;
                    if (enterAttr.Command != MsgDTO.command || enterAttr.SourceType != MsgType.Group)
                    {
                        continue;
                    }

                    string authority = CQ.GetGroupMemberInfo(MsgDTO.fromGroup, MsgDTO.fromQQ, true).Authority;
                    if(enterAttr.AuthorityLevel == AuthorityLevel.群主 && authority != "群主")
                    {
                        break;
                    }
                    if(enterAttr.AuthorityLevel == AuthorityLevel.管理员 && (authority != "群主" && authority != "管理员"))
                    {
                        break;
                    }

                    t.InvokeMember(method.Name,
                            BindingFlags.InvokeMethod,
                            null,
                            this,
                            new object[] { MsgDTO }
                            );
                    return;
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
                        continue;
                    }
                    if(enterAttr.IsDeveloperOnly && MsgDTO.fromQQ != Common.DeveloperNumber)
                    {
                        continue;
                    }
                    t.InvokeMember(method.Name,
                            BindingFlags.InvokeMethod,
                            null,
                            this,
                            new object[] { MsgDTO }
                            );
                    return;
                }
            }
        }
    }
}
