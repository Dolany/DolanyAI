using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Flexlive.CQP.Framework;
using Flexlive.CQP.Framework.Utils;

namespace AILib
{
    public abstract class AIBase : IComparable<AIBase>
    {
        public AIConfigDTO ConfigDTO { get; set; }
        public int PriorityLevel { get; set; }

        public abstract void Work();

        public AIBase(AIConfigDTO ConfigDTO)
        {
            this.ConfigDTO = ConfigDTO;
        }

        public virtual bool OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            if (MsgDTO.fromQQ < 0)
            {
                MsgDTO.fromQQ = MsgDTO.fromQQ & 0xFFFFFFFF;
            }

            Type t = this.GetType();
            foreach (var method in t.GetMethods())
            {
                foreach (var attr in method.GetCustomAttributes(typeof(EnterCommandAttribute), false))
                {
                    var enterAttr = attr as EnterCommandAttribute;
                    if (enterAttr.Command != MsgDTO.command || enterAttr.SourceType != MsgType.Group)
                    {
                        continue;
                    }

                    RuntimeLogger.Log($"try to get authority: fromGroup:{MsgDTO.fromGroup}, fromQQ:{MsgDTO.fromQQ}, time:{DateTime.Now}");
                    string authority = CQ.GetGroupMemberInfo(MsgDTO.fromGroup, MsgDTO.fromQQ, true).Authority;
                    RuntimeLogger.Log($"authority is {authority}");
                    if (!AuthorityCheck(enterAttr.AuthorityLevel, authority))
                    {
                        break;
                    }

                    t.InvokeMember(method.Name,
                            BindingFlags.InvokeMethod,
                            null,
                            this,
                            new object[] { MsgDTO }
                            );
                    return true;
                }
            }

            return false;
        }

        private bool AuthorityCheck(AuthorityLevel authorityLevel, string authority)
        {
            if (authorityLevel == AuthorityLevel.群主 && authority != "群主")
            {
                return false;
            }
            if (authorityLevel == AuthorityLevel.管理员 && (authority != "群主" && authority != "管理员"))
            {
                return false;
            }

            return true;
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
                    if (enterAttr.IsDeveloperOnly && MsgDTO.fromQQ != Common.DeveloperNumber)
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

        public int CompareTo(AIBase other)
        {
            return this.PriorityLevel.CompareTo(other.PriorityLevel);
        }
    }
}