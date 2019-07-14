using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Doremi.Cache;
using Dolany.Ai.Doremi.Common;
using Dolany.Database.Ai;

namespace Dolany.Ai.Doremi.Base
{
    public abstract class AIBase
    {
        protected delegate bool AIModuleDel(MsgInformationEx MsgDTO, object[] param);

        protected readonly Dictionary<EnterCommandAttribute, AIModuleDel> ModuleDels = new Dictionary<EnterCommandAttribute, AIModuleDel>();

        public readonly AIAttribute AIAttr;

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
                        ModuleDels.Add(attrClone, method.CreateDelegate(typeof(AIModuleDel), this) as AIModuleDel);
                    }
                }
            }

            AIAttr = t.GetCustomAttribute(typeof(AIAttribute), false) as AIAttribute;
        }

        public virtual void Initialization()
        {
        }

        public virtual bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            var query = ModuleDels.Where(c => c.Key.Command == MsgDTO.Command).ToList();
            if (query.IsNullOrEmpty())
            {
                return false;
            }

            try
            {
                foreach (var (enterCommandAttribute, moduleDel) in query)
                {
                    if (!Check(enterCommandAttribute, MsgDTO, out var param))
                    {
                        continue;
                    }

                    AIAnalyzer.AddCommandCount(new CommandAnalyzeDTO()
                    {
                        Ai = AIAttr.Name, Command = enterCommandAttribute.Command, GroupNum = MsgDTO.FromGroup, BindAi = MsgDTO.BindAi
                    });

                    if (!StateCheck(MsgDTO))
                    {
                        return true;
                    }

                    var limitRecord = DailyLimitRecord.Get(MsgDTO.FromQQ, enterCommandAttribute.ID);
                    var checkResult = DailyLimitCheck(enterCommandAttribute, MsgDTO, limitRecord);
                    if (!checkResult)
                    {
                        MsgSender.PushMsg(MsgDTO, $"你今天 {enterCommandAttribute.Command} 的次数已用完");
                        return true;
                    }

                    var result = moduleDel(MsgDTO, param);

                    if (!result)
                    {
                        return true;
                    }

                    limitRecord.Cache();
                    limitRecord.Update();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            return false;
        }

        private bool StateCheck(MsgInformationEx MsgDTO)
        {
            if (MsgDTO.Type != MsgType.Group)
            {
                return true;
            }

            var stateCache = AliveStateMgr.Instance.GetState(MsgDTO.FromGroup, MsgDTO.FromQQ);
            if (stateCache != null)
            {
                MsgSender.PushMsg(MsgDTO, $"你已经死了({stateCache.Name})！复活时间：{stateCache.RebornTime.ToString(CultureInfo.CurrentCulture)}", true);
                return false;
            }

            if (!AIAttr.NeedManulOpen || GroupSettingMgr.Instance[MsgDTO.FromGroup].HasFunction(AIAttr.Name))
            {
                return true;
            }

            MsgSender.PushMsg(MsgDTO, $"本群尚未开启 {AIAttr.Name} 功能，请联系群主使用 开启功能 命令来开启此功能！");
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

        private static bool DailyLimitCheck(EnterCommandAttribute enterAttr, MsgInformation MsgDTO, DailyLimitRecord limitRecord)
        {
            var isTestingGroup = Global.TestGroups.Contains(MsgDTO.FromGroup);
            var timesLimit = isTestingGroup ? enterAttr.TestingDailyLimit : enterAttr.DailyLimit;
            return timesLimit == 0 || limitRecord.Check(timesLimit);
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
                    var anyValue = string.Join(" ", paramStrs.Skip(i));
                    if (string.IsNullOrEmpty(anyValue.Trim()))
                    {
                        return false;
                    }

                    list.Add(anyValue);
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
            if (MsgDTO.Type == MsgType.Private)
            {
                return PrivateAuthCheck(enterAttr);
            }

            if (MsgDTO.Auth == AuthorityLevel.未知)
            {
                MsgDTO.Auth = Utility.GetAuth(MsgDTO);
            }

            return GroupAuthCheck(authorityLevel, MsgDTO);
        }

        private static bool GroupAuthCheck(AuthorityLevel authorityLevel, MsgInformationEx MsgDTO)
        {
            var auth = MsgDTO.Auth;

            if (auth == AuthorityLevel.开发者)
            {
                return true;
            }
            if (authorityLevel == AuthorityLevel.开发者)
            {
                return false;
            }
            if (auth == AuthorityLevel.群主)
            {
                return true;
            }
            if (authorityLevel == AuthorityLevel.群主)
            {
                return false;
            }
            if (auth == AuthorityLevel.管理员)
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
    }
}
