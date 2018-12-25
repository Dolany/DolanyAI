using System;
using System.Collections.Generic;

namespace Dolany.Ai.Core.Base
{
    using System.IO;
    using System.Linq;

    using Dolany.Ai.Core.Cache;
    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Db;

    using Microsoft.Extensions.Configuration;

    using static Dolany.Ai.Core.API.CodeApi;
    using static Dolany.Ai.Core.Common.Utility;

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
                        MsgSender.Instance.PushMsg(MsgDTO, Code_Image(new FileInfo("images/过热.jpg").FullName));
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
                    if (checker == null ||
                        !checker.Check(paramStrs[i], out var p))
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

        private static bool AuthorityCheck(
            AuthorityLevel authorityLevel,
            EnterCommandAttribute enterAttr,
            MsgInformationEx MsgDTO)
        {
            return MsgDTO.Type == MsgType.Group ?
                GroupAuthCheck(authorityLevel, MsgDTO) :
                PrivateAuthCheck(enterAttr);
        }

        private static string GetAuthName(MsgInformationEx MsgDTO)
        {
            var tempAuth = GetTempAuth(MsgDTO);
            if (MsgDTO.FromQQ == DeveloperNumber || tempAuth == "开发者")
            {
                return "开发者";
            }

            var mi = GetMemberInfo(MsgDTO);
            if (mi == null)
            {
                MsgSender.Instance.PushMsg(
                    MsgDTO, "获取权限信息失败！");
                return "成员";
            }

            var authority = mi.Role;
            if (authority == 0 || tempAuth == "群主")
            {
                return "群主";
            }

            if (authority == 1 || tempAuth == "管理员")
            {
                return "管理员";
            }

            return "成员";
        }

        private static bool GroupAuthCheck(AuthorityLevel authorityLevel, MsgInformationEx MsgDTO)
        {
            var authName = GetAuthName(MsgDTO);

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

        private static string GetTempAuth(MsgInformation MsgDTO)
        {
            using (var db = new AIDatabase())
            {
                var date = DateTime.Now.Date;
                var authInfo = db.TempAuthorize.FirstOrDefault(
                    t => t.GroupNum == MsgDTO.FromGroup && t.QQNum == MsgDTO.FromQQ && t.AuthDate == date);
                if (authInfo != null)
                {
                    return authInfo.AuthName;
                }
            }

            return string.Empty;
        }

        private static bool PrivateAuthCheck(EnterCommandAttribute enterAttr)
        {
            return enterAttr.IsPrivateAvailabe;
        }

        public virtual void OnActiveStateChange(bool state, long GroupNum)
        {
        }
    }
}
