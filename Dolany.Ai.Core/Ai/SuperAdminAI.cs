using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.Common.PicReview;
using Dolany.Database;
using Dolany.Database.Ai;
using Dolany.UtilityTool;

namespace Dolany.Ai.Core.Ai
{
    public class SuperAdminAI : AIBase
    {
        public override string AIName { get; set; } = "超管";
        public override string Description { get; set; } = "Ai for Super Admin.";
        public override AIPriority PriorityLevel { get;} = AIPriority.SuperHigh;
        protected override CmdTagEnum DefaultTag { get; } = CmdTagEnum.超管;

        public PicReviewSvc PicReviewSvc { get; set; }
        public DataRefreshSvc DataRefreshSvc { get; set; }
        public BindAiSvc BindAiSvc { get; set; }
        public DirtyFilterSvc DirtyFilterSvc { get; set; }
        public RestrictorSvc RestrictorSvc { get; set; }
        public CrossWorldAiSvc CrossWorldAiSvc { get; set; }

        [EnterCommand(ID = "SuperAdminAI_FunctionBonus",
            Command = "功能奖励",
            Description = "奖励某个人某个功能若个使用次数（当日有效）",
            SyntaxHint = "[命令名] [@QQ号] [奖励个数]",
            Tag = CmdTagEnum.GM奖励,
            SyntaxChecker = "Word At Long",
            AuthorityLevel = AuthorityLevel.开发者)]
        public bool FunctionBonus(MsgInformationEx MsgDTO, object[] param)
        {
            var command = param[0] as string;
            var qqNum = (long)param[1];
            var count = (int) (long) param[2];

            var enters = CrossWorldAiSvc[MsgDTO.FromGroup].AllAvailableGroupCommands.Where(p => p.Command == command).ToList();
            if (enters.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "未找到该功能！", true);
                return false;
            }

            var enter = enters.First();
            if (enters.Count > 1)
            {
                var options = enters.Select(p => $"{p.Command} {p.SyntaxHint}").ToArray();
                var response = WaiterSvc.WaitForOptions(MsgDTO.FromGroup, MsgDTO.FromQQ, "请选择需要奖励的功能：", options, MsgDTO.BindAi);
                if (response < 0)
                {
                    MsgSender.PushMsg(MsgDTO, "操作取消！");
                    return false;
                }

                enter = enters[response];
            }

            var dailyLimit = DailyLimitRecord.Get(qqNum, enter.ID);
            dailyLimit.Decache(count);
            dailyLimit.Update();

            MsgSender.PushMsg(MsgDTO, "奖励已生效！");
            return true;
        }

        [EnterCommand(ID = "SuperAdminAI_BlackList",
            Command = "BlackList 黑名单",
            Description = "Put someone to blacklist",
            SyntaxHint = "[@QQ号]",
            SyntaxChecker = "At",
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

            DirtyFilterSvc.RefreshData();

            MsgSender.PushMsg(MsgDTO, "Success");
            return true;
        }

        [EnterCommand(ID = "SuperAdminAI_FreeBlackList",
            Command = "FreeBlackList 解除黑名单",
            Description = "Pull someone out from blacklist",
            SyntaxHint = "[@QQ号]",
            SyntaxChecker = "At",
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

            DirtyFilterSvc.RefreshData();
            MsgSender.PushMsg(MsgDTO, "Success");
            return true;
        }

        [EnterCommand(ID = "SuperAdminAI_InitAi",
            Command = "初始化",
            Description = "初始化群成员信息",
            SyntaxHint = "[群号]",
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
            SyntaxHint = "[群号] [群名]",
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
                BindAis = new List<string>(){MsgDTO.BindAi},
                WorldLine = CrossWorldAiSvc.DefaultWorldLine.Name
            };
            MongoService<GroupSettings>.Insert(setting);
            GroupSettingSvc.RefreshData();

            MsgSender.PushMsg(MsgDTO, "注册成功！");
            return true;
        }

        [EnterCommand(ID = "SuperAdminAI_BindAi",
            Command = "绑定",
            Description = "绑定机器人",
            SyntaxHint = "[群号] [机器人名]",
            SyntaxChecker = "Long Word",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool BindAi(MsgInformationEx MsgDTO, object[] param)
        {
            var groupNum = (long) param[0];
            var name = (string) param[1];

            if (!GroupSettingSvc.SettingDic.ContainsKey(groupNum))
            {
                MsgSender.PushMsg(MsgDTO, "错误的群号");
                return false;
            }

            if (!BindAiSvc.AiDic.ContainsKey(name))
            {
                MsgSender.PushMsg(MsgDTO, "错误的机器人名");
                return false;
            }

            var setting = GroupSettingSvc[groupNum];
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
            SyntaxHint = "[群组号]",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool Freeze(MsgInformationEx MsgDTO, object[] param)
        {
            var groupNum = (long) param[0];
            if (!GroupSettingSvc.SettingDic.ContainsKey(groupNum))
            {
                MsgSender.PushMsg(MsgDTO, "未找到相关群组");
                return false;
            }

            var setting = GroupSettingSvc[groupNum];
            setting.ForcedShutDown = true;
            setting.Update();

            MsgSender.PushMsg(MsgDTO, "冻结成功");

            return true;
        }

        [EnterCommand(ID = "SuperAdminAI_Defreeze",
            Command = "解冻",
            Description = "解冻某个群的机器人",
            SyntaxHint = "[群组号]",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool Defreeze(MsgInformationEx MsgDTO, object[] param)
        {
            var groupNum = (long) param[0];
            if (!GroupSettingSvc.SettingDic.ContainsKey(groupNum))
            {
                MsgSender.PushMsg(MsgDTO, "未找到相关群组");
                return false;
            }

            var setting = GroupSettingSvc[groupNum];
            setting.ForcedShutDown = false;
            setting.Update();

            MsgSender.PushMsg(MsgDTO, "解冻成功");

            return true;
        }

        [EnterCommand(ID = "SuperAdminAI_ChargeTime",
            Command = "充值时间",
            Description = "给某个群组充值时间(单位天)",
            SyntaxHint = "[群组号] [天数]",
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

            GroupSettingSvc.RefreshData();

            MsgSender.PushMsg(MsgDTO, $"充值成功，有效期至 {setting.ExpiryTime:yyyy-MM-dd HH:mm:ss}");
            return true;
        }

        [EnterCommand(ID = "SuperAdminAI_EmergencyUnload",
            Command = "紧急卸载",
            Description = "紧急停用某个机器人，取消其所有群组的绑定",
            SyntaxHint = "[机器人名]",
            SyntaxChecker = "Word",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool EmergencyUnload(MsgInformationEx MsgDTO, object[] param)
        {
            var bingAiName = param[0] as string;
            if (BindAiSvc[bingAiName] == null)
            {
                MsgSender.PushMsg(MsgDTO, "未找到该机器人！");
                return false;
            }

            var failedGroups = new List<GroupSettings>();
            var successGroups = new List<GroupSettings>();
            foreach (var setting in GroupSettingSvc.SettingDic.Values.Where(setting => !setting.BindAis.IsNullOrEmpty())
                .Where(setting => setting.BindAis.Contains(bingAiName)))
            {
                if (setting.BindAis.Count == 1)
                {
                    failedGroups.Add(setting);
                    continue;
                }

                setting.BindAis = setting.BindAis.Where(p => p != bingAiName).ToList();
                if (setting.BindAi == bingAiName)
                {
                    setting.BindAi = setting.BindAis.RandElement();
                }

                setting.Update();
                successGroups.Add(setting);
            }

            var msg = "卸载完毕！";
            if (!successGroups.IsNullOrEmpty())
            {
                msg += $"\r\n以下群组卸载成功！\r\n{string.Join(",", successGroups.Select(p => p.Name))}";
            }
            if (!failedGroups.IsNullOrEmpty())
            {
                msg += $"\r\n以下群组卸载失败，请手动处理！\r\n{string.Join(",", failedGroups.Select(p => p.Name))}";
            }

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "SuperAdminAI_ForbiddenPicCache",
            Command = "禁用图片缓存",
            Description = "禁用一个群的图片缓存",
            SyntaxHint = "[群号]",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool ForbiddenPicCache(MsgInformationEx MsgDTO, object[] param)
        {
            var groupNum = (long) param[0];
            var groupSetting = GroupSettingSvc[groupNum];
            if (groupSetting.AdditionSettings == null)
            {
                groupSetting.AdditionSettings = new Dictionary<string, string>();
            }
            groupSetting.AdditionSettings.AddSafe("禁止图片缓存", true.ToString());
            groupSetting.Update();

            MsgSender.PushMsg(MsgDTO, "命令已完成！");
            return true;
        }

        [EnterCommand(ID = "SuperAdminAI_PicReview",
            Command = "图片审核",
            Description = "审核一张图片",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true,
            IsGroupAvailable = true)]
        public bool PicReview(MsgInformationEx MsgDTO, object[] param)
        {
            PicReviewSvc.Review(MsgDTO);
            return true;
        }

        [EnterCommand(ID = "SuperAdminAI_DataRefresh",
            Command = "刷新数据 数据刷新",
            Description = "刷新所有数据信息",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool DataRefresh(MsgInformationEx MsgDTO, object[] param)
        {
            var count = DataRefreshSvc.RefreshAll();
            MsgSender.PushMsg(MsgDTO, $"刷新成功！共刷新 {count}个数据项！");
            return true;
        }

        [EnterCommand(ID = "SuperAdminAI_SysPressure",
            Command = "系统压力",
            Description = "查看系统压力信息",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool SysPressure(MsgInformationEx MsgDTO, object[] param)
        {
            var msg = string.Join("\r\n",
                RestrictorSvc.Pressures.Select(p => $"{p.Key}:{p.Value}/{RestrictorSvc.BindAiLimit[p.Key]}{(RestrictorSvc.IsTooFreq(p.Key) ? "(危)" : string.Empty)}"));
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "SuperAdminAI_CurrentRestrictor",
            Command = "现行限流方案",
            Description = "查看当前的限流方案",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool CurrentRestrictor(MsgInformationEx MsgDTO, object[] param)
        {
            var limitDic = RestrictorSvc.BindAiLimit;
            MsgSender.PushMsg(MsgDTO, string.Join("\r\n", limitDic.Select(p => $"{p.Key}:{p.Value}")));
            return true;
        }

        [EnterCommand(ID = "SuperAdminAI_SetRestrictor",
            Command = "设置限流 设定限流",
            Description = "设置某个机器人的限流方案",
            SyntaxHint = "[机器人名] [限流上限]",
            SyntaxChecker = "Word Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool SetRestrictor(MsgInformationEx MsgDTO, object[] param)
        {
            var name = (string) param[0];
            var limit = (int) (long) param[1];

            if (BindAiSvc[name] == null)
            {
                MsgSender.PushMsg(MsgDTO, "未识别到该机器人！");
                return false;
            }

            var rec = BindAiRestrict.Get(name);
            rec.MaxLimit = limit;
            rec.Update();

            RestrictorSvc.RefreshData();
            MsgSender.PushMsg(MsgDTO, "设定成功！");
            return true;
        }
    }
}
