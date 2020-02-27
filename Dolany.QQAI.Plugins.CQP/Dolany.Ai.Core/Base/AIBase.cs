using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Database.Ai;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Dolany.Ai.Core.SyntaxChecker;

namespace Dolany.Ai.Core.Base
{
    public abstract class AIBase : IDependency
    {
        public abstract string AIName { get; set; }

        public abstract string Description { get; set; }

        public virtual AIPriority PriorityLevel { get; } = AIPriority.Normal;

        public virtual bool Enable { get; } = true;

        public virtual bool NeedManualOpeon { get; } = false;

        protected delegate bool AIModuleDel(MsgInformationEx MsgDTO, object[] param);

        protected readonly Dictionary<EnterCommandAttribute, AIModuleDel> ModuleDels = new Dictionary<EnterCommandAttribute, AIModuleDel>();

        public IWorldLine WorldLine { get; set; }

        public WaiterSvc WaiterSvc { get; set; }
        public GroupSettingSvc GroupSettingSvc { get; set; }
        public AliveStateMgr AliveStateMgr { get; set; }
        public CommandLocker CommandLocker { get; set; }
        public SyntaxCheckerMgr SyntaxCheckerMgr { get; set; }

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

                    AIAnalyzer.AddCommandCount(new CmdRec()
                    {
                        FunctionalAi = AIName,
                        Command = enterCommandAttribute.Command,
                        GroupNum = MsgDTO.FromGroup,
                        BindAi = MsgDTO.BindAi
                    });

                    if (!StateCheck(MsgDTO))
                    {
                        return true;
                    }

                    var limitRecord = DailyLimitRecord.Get(MsgDTO.FromQQ, enterCommandAttribute.ID);
                    var checkResult = DailyLimitCheck(enterCommandAttribute, MsgDTO, limitRecord);
                    if (!checkResult)
                    {
                        MsgSender.PushMsg(MsgDTO, $"你今天 【{enterCommandAttribute.Command}】 的次数已用完");
                        return true;
                    }

                    if (!CommandLocker.Check(MsgDTO.FromQQ, enterCommandAttribute.ID))
                    {
                        return false;
                    }

                    var lockID = CommandLocker.Lock(MsgDTO.FromQQ, new[] {enterCommandAttribute.ID});
                    var result = moduleDel(MsgDTO, param);
                    CommandLocker.FreeLock(lockID);

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

        public virtual bool OnMoneyReceived(ChargeModel model)
        {
            return false;
        }

        public virtual bool OnGroupMemberChanged(GroupMemberChangeModel model)
        {
            return false;
        }

        private bool StateCheck(MsgInformationEx MsgDTO)
        {
            if (MsgDTO.Type != MsgType.Group)
            {
                return true;
            }

            if (!MsgDTO.IsAlive)
            {
                var stateCache = AliveStateMgr.GetState(MsgDTO.FromGroup, MsgDTO.FromQQ);
                MsgSender.PushMsg(MsgDTO, $"你已经死了({stateCache.Name})！复活时间：{stateCache.RebornTime.ToString(CultureInfo.CurrentCulture)}", true);
                return false;
            }

            if (!NeedManualOpeon || GroupSettingSvc[MsgDTO.FromGroup].HasFunction(AIName))
            {
                return true;
            }

            MsgSender.PushMsg(MsgDTO, $"本群尚未开启 【{AIName}】 功能，请联系群主使用 【开启功能】 命令来开启此功能！");
            return false;
        }

        private bool Check(EnterCommandAttribute enterAttr, MsgInformationEx MsgDTO, out object[] param)
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

            if (!SyntaxCheckerMgr.SyntaxCheck(enterAttr.SyntaxChecker, MsgDTO.Msg, out param))
            {
                return false;
            }

            if (MsgDTO.Type == MsgType.Private)
            {
                return true;
            }

            if (!AuthorityCheck(enterAttr.AuthorityLevel, enterAttr, MsgDTO))
            {
                MsgSender.PushMsg(MsgDTO, $"权限不足！需要 【{enterAttr.AuthorityLevel.ToString()}】 权限！");
                return false;
            }

            return !enterAttr.IsTesting || Global.TestGroups.Contains(MsgDTO.FromGroup);
        }

        private static bool DailyLimitCheck(EnterCommandAttribute enterAttr, MsgInformation MsgDTO, DailyLimitRecord limitRecord)
        {
            var isTestingGroup = Global.TestGroups.Contains(MsgDTO.FromGroup);
            var timesLimit = isTestingGroup ? enterAttr.TestingDailyLimit : enterAttr.DailyLimit;
            return timesLimit == 0 || limitRecord.Check(timesLimit);
        }

        private static bool AuthorityCheck(AuthorityLevel authorityLevel, EnterCommandAttribute enterAttr, MsgInformationEx MsgDTO)
        {
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

            return authorityLevel != AuthorityLevel.管理员;
        }
    }
}
