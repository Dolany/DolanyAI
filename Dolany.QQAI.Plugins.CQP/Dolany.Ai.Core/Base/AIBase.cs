using System.Reflection;

namespace Dolany.Ai.Core.Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using API;

    using Cache;

    using Common;

    using Dolany.Ai.Common;
    using Database.Sqlite;
    using Dolany.Database.Sqlite.Model;

    using Model;

    public abstract class AIBase
    {
        protected delegate void MsgConsolerDel(MsgInformationEx msgDTO, object[] para);

        protected readonly Dictionary<EnterCommandAttribute, MsgConsolerDel> Consolers =
            new Dictionary<EnterCommandAttribute, MsgConsolerDel>();

        protected readonly AIAttribute Attr;

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

            Attr = t.GetCustomAttribute(typeof(AIAttribute), false) as AIAttribute;
        }

        public virtual void Initialization()
        {
        }

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
                        MsgSender.Instance.PushMsg(MsgDTO, CodeApi.Code_Image_Relational("images/过热.jpg"));
                        return true;
                    }

                    if (MsgDTO.Type == MsgType.Group && Attr.NeedManulOpen && !AIStateMgr.Instance.GetState(Attr.Name, MsgDTO.FromGroup))
                    {
                        MsgSender.Instance.PushMsg(MsgDTO, $"本群尚未开启 {Attr.Name} 功能，请联系群主开启此功能，或者添加冰冰酱好友后使用私聊命令，或者申请加入AI测试群！");
                        return true;
                    }

                    if (!DailyLimitCheck(consoler.Key, MsgDTO))
                    {
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

            if (enterAttr.IsTesting && !Global.TestGroups.Contains(MsgDTO.FromGroup))
            {
                return false;
            }

            return true;
        }

        private static bool DailyLimitCheck(EnterCommandAttribute enterAttr, MsgInformationEx MsgDTO)
        {
            var isTestingGroup = Global.TestGroups.Contains(MsgDTO.FromGroup);
            if ((isTestingGroup && enterAttr.TestingDailyLimit == 0) || (!isTestingGroup && enterAttr.DailyLimit == 0))
            {
                return true;
            }

            var key = $"DailyLimit-{enterAttr.Command}-{MsgDTO.FromQQ}";
            var cache = SCacheService.Get<DailyLimitCache>(key);

            if (cache == null)
            {
                SCacheService.Cache(key, new DailyLimitCache { Count = 1, QQNum = MsgDTO.FromQQ, Command = enterAttr.Command });
            }
            else
            {
                if ((isTestingGroup && cache.Count >= enterAttr.TestingDailyLimit) || (!isTestingGroup && cache.Count >= enterAttr.DailyLimit))
                {
                    MsgSender.Instance.PushMsg(MsgDTO, $"今天 {enterAttr.Command} 的次数已用完，请明天再试~", true);
                    return false;
                }

                cache.Count++;
                SCacheService.Cache(key, cache);
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

            var checkers = SyntaxChecker.Split(' ');
            var paramStrs = msg.Split(' ');

            if (checkers.Length > paramStrs.Length)
            {
                return false;
            }

            if (!checkers.Contains("Any") && checkers.Length < paramStrs.Length)
            {
                return false;
            }

            var list = new List<object>();
            for (var i = 0; i < checkers.Length; i++)
            {
                var checker = AIMgr.Instance.Checkers.FirstOrDefault(c => c.Name == checkers[i]);

                if (checker == null)
                {
                    return false;
                }

                if (checker.Name == "Any")
                {
                    list.Add(string.Join(" ", paramStrs.Skip(i)));
                    break;
                }

                if (!checker.Check(paramStrs[i], out var p))
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

        private static bool AuthorityCheck(AuthorityLevel authorityLevel, EnterCommandAttribute enterAttr, MsgInformationEx MsgDTO)
        {
            if (string.IsNullOrEmpty(MsgDTO.AuthName))
            {
                MsgDTO.AuthName = Utility.GetAuthName(MsgDTO);
            }

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
