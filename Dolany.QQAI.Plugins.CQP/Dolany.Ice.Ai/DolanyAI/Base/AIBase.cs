using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Dolany.Ice.Ai.DolanyAI
{
    public abstract class AIBase
    {
        // ReSharper disable once MemberCanBeProtected.Global
        public delegate void MsgConsolerDel(ReceivedMsgDTO msgDTO, object[] para);

        // ReSharper disable once MemberCanBePrivate.Global
        protected readonly Dictionary<EnterCommandAttribute, MsgConsolerDel> Consolers =
            new Dictionary<EnterCommandAttribute, MsgConsolerDel>();

        protected AIBase()
        {
            var t = GetType();
            foreach (var method in t.GetMethods())
            {
                foreach (EnterCommandAttribute attr in method.GetCustomAttributes(typeof(EnterCommandAttribute), false))
                {
                    foreach (var command in attr.CommandsList)
                    {
                        var attrClone = attr.Clone();
                        attrClone.Command = command;
                        Consolers.Add(attrClone, method.CreateDelegate(typeof(MsgConsolerDel), this) as MsgConsolerDel);
                    }
                }
            }
        }

        public abstract void Work();

        public virtual bool OnMsgReceived(ReceivedMsgDTO MsgDTO)
        {
            var query = Consolers.Where(c => c.Key.Command == MsgDTO.Command);
            if (query.IsNullOrEmpty())
            {
                return false;
            }

            try
            {
                foreach (var consoler in query)
                {
                    if (!Check(consoler.Key, MsgDTO, out var param))
                    {
                        continue;
                    }

                    consoler.Value(MsgDTO, param);
                    return true;
                }
            }
            catch (Exception ex)
            {
                RuntimeLogger.Log(ex);
            }

            return false;
        }

        private static bool Check(EnterCommandAttribute enterAttr, ReceivedMsgDTO MsgDTO, out object[] param)
        {
            param = null;
            if (!enterAttr.CommandsList.Contains(MsgDTO.Command))
            {
                return false;
            }

            if (!SyntaxCheck(enterAttr.SyntaxChecker, MsgDTO.Msg, out param))
            {
                return false;
            }

            if (!AuthorityCheck(enterAttr.AuthorityLevel, enterAttr, MsgDTO))
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

        private static bool AuthorityCheck(AuthorityLevel authorityLevel, EnterCommandAttribute enterAttr, ReceivedMsgDTO MsgDTO)
        {
            if (MsgDTO.FromQQ == Utility.DeveloperNumber)
            {
                return true;
            }

            if (MsgDTO.MsgType == MsgType.Group)
            {
                return GroupCheck(authorityLevel, MsgDTO);
            }

            return PrivateCheck(enterAttr);
        }

        private static bool GroupCheck(AuthorityLevel authorityLevel, ReceivedMsgDTO MsgDTO)
        {
            var mi = Utility.GetMemberInfo(MsgDTO);
            if (mi == null)
            {
                return false;
            }

            var authority = mi.Role;
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

        private static bool PrivateCheck(EnterCommandAttribute enterAttr)
        {
            if (!enterAttr.IsPrivateAvailabe)
            {
                return false;
            }

            return true;
        }
    }
}