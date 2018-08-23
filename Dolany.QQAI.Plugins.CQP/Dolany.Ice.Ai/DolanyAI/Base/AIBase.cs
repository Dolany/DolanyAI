using System;
using System.Diagnostics;
using System.Reflection;

namespace Dolany.Ice.Ai.DolanyAI
{
    public abstract class AIBase
    {
        public abstract void Work();

        public virtual bool OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            var t = GetType();
            foreach (var method in t.GetMethods())
            {
                foreach (var attr in method.GetCustomAttributes(typeof(GroupEnterCommandAttribute), false))
                {
                    if (!GroupCheck(attr as GroupEnterCommandAttribute, MsgDTO, out object[] param))
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
            param = null;
            if (enterAttr.Command != MsgDTO.Command)
            {
                return false;
            }

            if (!SyntaxCheck(enterAttr.SyntaxChecker, MsgDTO.Msg, out param))
            {
                return false;
            }

            var mi = Utility.GetMemberInfo(MsgDTO);
            if (mi == null)
            {
                return false;
            }

            var authority = mi.Role;
            if (!AuthorityCheck(enterAttr.AuthorityLevel, authority, MsgDTO.FromQQ))
            {
                return false;
            }

            return true;
        }

        private static bool SyntaxCheck(string SyntaxChecker, string msg, out object[] param)
        {
            if (string.IsNullOrEmpty(SyntaxChecker))
            {
                param = null;
                return true;
            }

            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var scObj = assembly.CreateInstance("Dolany.Ice.Ai.DolanyAI." + SyntaxChecker + "Checker");
                var checker = scObj as ISyntaxChecker;
                Debug.Assert(checker != null, nameof(checker) + " != null");
                return checker.Check(msg, out param);
            }
            catch (Exception)
            {
                param = null;
                return false;
            }
        }

        private static bool AuthorityCheck(AuthorityLevel authorityLevel, int authority, long QQNum)
        {
            if (QQNum == Utility.DeveloperNumber)
            {
                return true;
            }
            if (authorityLevel == AuthorityLevel.开发者)
            {
                return false;
            }
            if (authority == 0)
            {
                return true;
            }
            if (authorityLevel == AuthorityLevel.群主)
            {
                return false;
            }
            if (authority == 1)
            {
                return true;
            }
            if (authorityLevel == AuthorityLevel.管理员)
            {
                return false;
            }

            return true;
        }

        public virtual void OnPrivateMsgReceived(PrivateMsgDTO MsgDTO)
        {
            var t = GetType();
            foreach (var method in t.GetMethods())
            {
                foreach (var attr in method.GetCustomAttributes(typeof(PrivateEnterCommandAttribute), false))
                {
                    if (!PrivateCheck(attr as PrivateEnterCommandAttribute, MsgDTO, out object[] param))
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

        private static bool PrivateCheck(PrivateEnterCommandAttribute enterAttr, PrivateMsgDTO MsgDTO, out object[] param)
        {
            param = null;
            if (enterAttr.Command != MsgDTO.Command)
            {
                return false;
            }

            if (enterAttr.IsDeveloperOnly && MsgDTO.FromQQ != Utility.DeveloperNumber)
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