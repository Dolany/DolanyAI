namespace Dolany.Ai.Core.Base
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using API;
    using Cache;
    using Common;

    using Dolany.Ai.Common;

    using Model;

    public abstract class AIBase
    {
        protected delegate void MsgConsolerDel(MsgInformationEx msgDTO, object[] para);

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

        public virtual bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            var query = Consolers.Where(c => c.Key.Command == MsgDTO.Command).ToList();
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

                    if (RecentCommandCache.IsTooFreq())
                    {
                        MsgSender.Instance.PushMsg(MsgDTO, "哇哇哇~~，AI过热中......");
                        MsgSender.Instance.PushMsg(MsgDTO, CodeApi.Code_Image(new FileInfo("images/过热.jpg").FullName));
                        return true;
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

        private static bool Check(EnterCommandAttribute enterAttr, MsgInformationEx MsgDTO, out object[] param)
        {
            param = null;
            if (!enterAttr.CommandsList.Contains(MsgDTO.Command))
            {
                return false;
            }

            if ((!enterAttr.IsGroupAvailable && MsgDTO.Type == MsgType.Group) || (!enterAttr.IsPrivateAvailable && MsgDTO.Type == MsgType.Private))
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
            param = null;
            if (string.IsNullOrEmpty(SyntaxChecker))
            {
                return false;
            }

            try
            {
                var checkers = SyntaxChecker.Split(' ');
                var paramStrs = msg.Split(' ');
                if (checkers.Length != paramStrs.Length)
                {
                    return false;
                }

                var list = new List<object>();
                for (var i = 0; i < checkers.Length; i++)
                {
                    var checker = AIMgr.Instance.Checkers.FirstOrDefault(c => c.Name == checkers[i]);
                    if (checker == null || !checker.Check(paramStrs[i], out var p))
                    {
                        return false;
                    }

                    if (p != null)
                    {
                        list.AddRange(p);
                    }
                }

                param = list.ToArray();
                return true;
            }
            catch (Exception ex)
            {
                RuntimeLogger.Log(ex);
                return false;
            }
        }

        private static bool AuthorityCheck(AuthorityLevel authorityLevel, EnterCommandAttribute enterAttr, MsgInformationEx MsgDTO)
        {
            return MsgDTO.Type == MsgType.Group ? GroupAuthCheck(authorityLevel, MsgDTO) : PrivateAuthCheck(enterAttr);
        }

        private static bool GroupAuthCheck(AuthorityLevel authorityLevel, MsgInformationEx MsgDTO)
        {
            var authName = MsgDTO.AuthName;

            if (authName == "开发者")
            {
                return true;
            }
            if (authorityLevel == AuthorityLevel.开发者)
            {
                return false;
            }
            if (authName == "群主")
            {
                return true;
            }
            if (authorityLevel == AuthorityLevel.群主)
            {
                return false;
            }
            if (authName == "管理员")
            {
                return true;
            }
            if (authorityLevel == AuthorityLevel.管理员)
            {
                return false;
            }

            return true;
        }

        private static bool PrivateAuthCheck(EnterCommandAttribute enterAttr)
        {
            return enterAttr.IsPrivateAvailable;
        }

        public virtual void OnActiveStateChange(bool state, long GroupNum)
        {
        }
    }
}
