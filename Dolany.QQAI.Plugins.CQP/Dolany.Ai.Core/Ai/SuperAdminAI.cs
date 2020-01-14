﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Ai
{
    public class SuperAdminAI : AIBase
    {
        public override string AIName { get; set; } = "超管";
        public override string Description { get; set; } = "Ai for Super Admin.";
        public override int PriorityLevel { get; set; } = 50;

        [EnterCommand(ID = "SuperAdminAI_FishingBonus",
            Command = "功能奖励",
            Description = "奖励某个人某个功能若个使用次数（当日有效）",
            Syntax = "[命令名] [@QQ号] [奖励个数]",
            Tag = "超管",
            SyntaxChecker = "Word At Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = false)]
        public bool FishingBonus(MsgInformationEx MsgDTO, object[] param)
        {
            var command = param[0] as string;
            var qqNum = (long)param[1];
            var count = (int) (long) param[2];

            var enter = WorldLine.AllAvailableGroupCommands.FirstOrDefault(p => p.CommandsList.Contains(command));
            if (enter == null)
            {
                MsgSender.PushMsg(MsgDTO, "未找到该功能！", true);
                return false;
            }

            var dailyLimit = DailyLimitRecord.Get(qqNum, enter.ID);
            dailyLimit.Decache(count);
            dailyLimit.Update();

            MsgSender.PushMsg(MsgDTO, "奖励已生效！");
            return true;
        }

        [EnterCommand(ID = "SuperAdminAI_BlackList",
            Command = "BlackList",
            Description = "Put someone to blacklist",
            Syntax = "qqnum",
            Tag = "超管",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool BlackList(MsgInformationEx MsgDTO, object[] param)
        {
            var qqNum = (long) param[0];
            var query = MongoService<BlackList>.GetOnly(b => b.QQNum == qqNum);
            if (query == null)
            {
                MongoService<BlackList>.Insert(new BlackList{QQNum = qqNum, BlackCount = 10, UpdateTime = DateTime.Now});
            }
            else
            {
                query.BlackCount = 10;
                MongoService<BlackList>.Update(query);
            }

            DirtyFilter.Instance.Refresh();

            MsgSender.PushMsg(MsgDTO, "Success");
            return true;
        }

        [EnterCommand(ID = "SuperAdminAI_FreeBlackList",
            Command = "FreeBlackList",
            Description = "Pull someone out from blacklist",
            Syntax = "qqnum",
            Tag = "超管",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool FreeBlackList(MsgInformationEx MsgDTO, object[] param)
        {
            var qqNum = (long) param[0];
            var query = MongoService<BlackList>.GetOnly(b => b.QQNum == qqNum);
            if (query == null)
            {
                MsgSender.PushMsg(MsgDTO, "Not In BlackList");
                return false;
            }

            MongoService<BlackList>.Delete(query);

            DirtyFilter.Instance.Refresh();
            MsgSender.PushMsg(MsgDTO, "Success");
            return true;
        }

        [EnterCommand(ID = "SuperAdminAI_InitAi",
            Command = "初始化",
            Description = "初始化群成员信息",
            Syntax = "[群号]",
            Tag = "超管",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool InitAi(MsgInformationEx MsgDTO, object[] param)
        {
            var groupNum = (long) param[0];
            if (!GroupMemberInfoCacher.RefreshGroupInfo(groupNum, MsgDTO.BindAi))
            {
                MsgSender.PushMsg(MsgDTO, "初始化失败，请稍后再试！");
                return false;
            }

            MsgSender.PushMsg(MsgDTO, "初始化成功！");
            return true;
        }

        [EnterCommand(ID = "SuperAdminAI_Register",
            Command = "注册",
            Description = "注册新的群组",
            Syntax = "[群号] [群名]",
            Tag = "超管",
            SyntaxChecker = "Long Word",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool Register(MsgInformationEx MsgDTO, object[] param)
        {
            var groupNum = (long) param[0];
            var name = param[1] as string;

            MongoService<GroupSettings>.DeleteMany(r => r.GroupNum == groupNum);
            var setting = new GroupSettings()
            {
                GroupNum = groupNum,
                Name = name,
                BindAi = MsgDTO.BindAi,
                BindAis = new List<string>(){MsgDTO.BindAi}
            };
            MongoService<GroupSettings>.Insert(setting);
            GroupSettingMgr.Instance.Refresh();

            MsgSender.PushMsg(MsgDTO, "注册成功！");
            return true;
        }

        [EnterCommand(ID = "SuperAdminAI_BindAi",
            Command = "绑定",
            Description = "绑定机器人",
            Syntax = "[群号] [机器人名]",
            Tag = "超管",
            SyntaxChecker = "Long Word",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool BindAi(MsgInformationEx MsgDTO, object[] param)
        {
            var groupNum = (long) param[0];
            var name = param[1] as string;

            if (!GroupSettingMgr.Instance.SettingDic.ContainsKey(groupNum))
            {
                MsgSender.PushMsg(MsgDTO, "错误的群号");
                return false;
            }

            if (!BindAiMgr.Instance.AiDic.ContainsKey(name))
            {
                MsgSender.PushMsg(MsgDTO, "错误的机器人名");
                return false;
            }

            var setting = GroupSettingMgr.Instance[groupNum];
            if (setting.BindAis == null)
            {
                setting.BindAis = new List<string>();
            }

            if (!setting.BindAis.Contains(name))
            {
                setting.BindAis.Add(name);
            }

            setting.Update();

            MsgSender.PushMsg(MsgDTO, "绑定成功！");
            return true;
        }

        [EnterCommand(ID = "SuperAdminAI_Freeze",
            Command = "冻结",
            Description = "冻结某个群的机器人",
            Syntax = "[群组号]",
            Tag = "超管",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool Freeze(MsgInformationEx MsgDTO, object[] param)
        {
            var groupNum = (long) param[0];
            if (!GroupSettingMgr.Instance.SettingDic.ContainsKey(groupNum))
            {
                MsgSender.PushMsg(MsgDTO, "未找到相关群组");
                return false;
            }

            var setting = GroupSettingMgr.Instance[groupNum];
            setting.ForcedShutDown = true;
            setting.Update();

            MsgSender.PushMsg(MsgDTO, "冻结成功");

            return true;
        }

        [EnterCommand(ID = "SuperAdminAI_Defreeze",
            Command = "解冻",
            Description = "解冻某个群的机器人",
            Syntax = "[群组号]",
            Tag = "超管",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool Defreeze(MsgInformationEx MsgDTO, object[] param)
        {
            var groupNum = (long) param[0];
            if (!GroupSettingMgr.Instance.SettingDic.ContainsKey(groupNum))
            {
                MsgSender.PushMsg(MsgDTO, "未找到相关群组");
                return false;
            }

            var setting = GroupSettingMgr.Instance[groupNum];
            setting.ForcedShutDown = false;
            setting.Update();

            MsgSender.PushMsg(MsgDTO, "解冻成功");

            return true;
        }

        [EnterCommand(ID = "SuperAdminAI_ChargeTime",
            Command = "充值时间",
            Description = "给某个群组充值时间(单位天)",
            Syntax = "[群组号] [天数]",
            Tag = "超管",
            SyntaxChecker = "Long Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool ChargeTime(MsgInformationEx MsgDTO, object[] param)
        {
            var groupNum = (long) param[0];
            var days = (int) (long) param[1];

            var setting = MongoService<GroupSettings>.GetOnly(p => p.GroupNum == groupNum);
            if (setting.ExpiryTime == null || setting.ExpiryTime.Value < DateTime.Now)
            {
                setting.ExpiryTime = DateTime.Now.AddDays(days);
            }
            else
            {
                setting.ExpiryTime = setting.ExpiryTime.Value.AddDays(days);
            }
            setting.Update();

            GroupSettingMgr.Instance.Refresh();

            MsgSender.PushMsg(MsgDTO, $"充值成功，有效期至 {setting.ExpiryTime?.ToLocalTime():yyyy-MM-dd HH:mm:ss}");
            return true;
        }
    }
}
