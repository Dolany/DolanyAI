/*已迁移*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Flexlive.CQP.Framework;
using Flexlive.CQP.Framework.Utils;
using AILib.SyntaxChecker;

namespace AILib
{
    public abstract class AIBase
    {
        public abstract void Work();

        public AIBase()
        {
        }

        public virtual bool OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            Type t = this.GetType();
            foreach (var method in t.GetMethods())
            {
                foreach (var attr in method.GetCustomAttributes(typeof(GroupEnterCommandAttribute), false))
                {
                    object[] param;

                    if (!GroupCheck(attr as GroupEnterCommandAttribute, MsgDTO, out param))
                    {
                        continue;
                    }

                    t.InvokeMember(method.Name,
                            BindingFlags.InvokeMethod,
                            null,
                            this,
                            new object[] { MsgDTO, param }
                            );
                    return true;
                }
            }

            return false;
        }

        private bool GroupCheck(GroupEnterCommandAttribute enterAttr, GroupMsgDTO MsgDTO, out object[] param)
        {
            if (enterAttr.Command != MsgDTO.Command)
            {
                param = null;
                return false;
            }

            if (!SyntaxCheck(enterAttr.SyntaxChecker, MsgDTO.Msg, out param))
            {
                return false;
            }

            string authority = CQ.GetGroupMemberInfo(MsgDTO.FromGroup, MsgDTO.FromQQ, true).Authority;
            if (!AuthorityCheck(enterAttr.AuthorityLevel, authority))
            {
                return false;
            }

            return true;
        }

        private bool SyntaxCheck(string SyntaxChecker, string msg, out object[] param)
        {
            if (string.IsNullOrEmpty(SyntaxChecker))
            {
                param = null;
                return true;
            }

            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                object scObj = assembly.CreateInstance("AILib.SyntaxChecker." + SyntaxChecker + "Checker");
                ISyntaxChecker checker = scObj as ISyntaxChecker;
                return checker.Check(msg, out param);
            }
            catch
            {
                param = null;
                return false;
            }
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
                foreach (var attr in method.GetCustomAttributes(typeof(PrivateEnterCommandAttribute), false))
                {
                    object[] param;

                    if (!PrivateCheck(attr as PrivateEnterCommandAttribute, MsgDTO, out param))
                    {
                        continue;
                    }

                    t.InvokeMember(method.Name,
                            BindingFlags.InvokeMethod,
                            null,
                            this,
                            new object[] { MsgDTO, param }
                            );
                    return;
                }
            }
        }

        private bool PrivateCheck(PrivateEnterCommandAttribute enterAttr, PrivateMsgDTO MsgDTO, out object[] param)
        {
            param = null;
            if (enterAttr.Command != MsgDTO.Command)
            {
                return false;
            }

            if (enterAttr.IsDeveloperOnly && MsgDTO.FromQQ != Common.DeveloperNumber)
            {
                return false;
            }

            if (!SyntaxCheck(enterAttr.SyntaxChecker, MsgDTO.Msg, out param))
            {
                return false;
            }

            return true;
        }
    }
}