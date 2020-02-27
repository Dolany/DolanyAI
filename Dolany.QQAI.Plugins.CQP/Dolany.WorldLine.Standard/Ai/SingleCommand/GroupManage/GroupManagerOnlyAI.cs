using System;
using System.Globalization;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Database;
using Dolany.WorldLine.Standard.OnlineStore;

namespace Dolany.WorldLine.Standard.Ai.SingleCommand.GroupManage
{
    public class GroupManagerOnlyAI : AIBase
    {
        public override string AIName { get; set; } = "死亡";

        public override string Description { get; set; } = "AI for killing somebody for some time.";

        public override AIPriority PriorityLevel { get;} = AIPriority.High;

        [EnterCommand(ID = "GroupManagerOnlyAI_DeathStaring",
            Command = "死亡凝视",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "让某个成员死亡(无法使用机器人)若干分钟（最高500分钟）",
            Syntax = "[@qq号] [分钟数]",
            Tag = "群管理",
            SyntaxChecker = "At Long",
            IsPrivateAvailable = false,
            DailyLimit = 4)]
        public bool DeathStaring(MsgInformationEx MsgDTO, object[] param)
        {
            var aimQQ = (long) param[0];
            var minutes = (int) (long) param[1];

            if (minutes <= 0 || minutes > 500)
            {
                MsgSender.PushMsg(MsgDTO, "错误的时间范围！");
                return false;
            }

            var key = $"AliveState-{MsgDTO.FromGroup}-{aimQQ}";
            if (!SkillCheck(MsgDTO, aimQQ, key))
            {
                return false;
            }

            var rebornTime = DateTime.Now.AddMinutes(minutes);
            DoSkill(MsgDTO, aimQQ, key, rebornTime, "死亡凝视");
            return true;
        }

        [EnterCommand(ID = "GroupManagerOnlyAI_StarLightBreak",
            Command = "星光爆裂",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "让某个成员死亡(无法使用机器人)若干小时（最高80小时）",
            Syntax = "[@qq号] [小时数]",
            Tag = "群管理",
            SyntaxChecker = "At Long",
            IsPrivateAvailable = false,
            DailyLimit = 3)]
        public bool StarLightBreak(MsgInformationEx MsgDTO, object[] param)
        {
            var aimQQ = (long) param[0];
            var hours = (int) (long) param[1];

            if (hours <= 0 || hours > 80)
            {
                MsgSender.PushMsg(MsgDTO, "错误的时间范围！");
                return false;
            }

            var key = $"AliveState-{MsgDTO.FromGroup}-{aimQQ}";
            if (!SkillCheck(MsgDTO, aimQQ, key))
            {
                return false;
            }

            var rebornTime = DateTime.Now.AddHours(hours);
            DoSkill(MsgDTO, aimQQ, key, rebornTime, "星光爆裂");
            return true;
        }

        [EnterCommand(ID = "GroupManagerOnlyAI_DreamSeal",
            Command = "梦想封印",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "让某个成员死亡(无法使用机器人)若干天（最高30天）",
            Syntax = "[@qq号] [天数]",
            Tag = "群管理",
            SyntaxChecker = "At Long",
            IsPrivateAvailable = false,
            DailyLimit = 2)]
        public bool DreamSeal(MsgInformationEx MsgDTO, object[] param)
        {
            var aimQQ = (long) param[0];
            var days = (int) (long) param[1];

            if (days <= 0 || days > 30)
            {
                MsgSender.PushMsg(MsgDTO, "错误的时间范围！");
                return false;
            }

            var key = $"AliveState-{MsgDTO.FromGroup}-{aimQQ}";
            if (!SkillCheck(MsgDTO, aimQQ, key))
            {
                return false;
            }

            var rebornTime = DateTime.Now.AddDays(days);
            DoSkill(MsgDTO, aimQQ, key, rebornTime, "梦想封印");
            return true;
        }

        [EnterCommand(ID = "GroupManagerOnlyAI_Reborn",
            Command = "复活",
            AuthorityLevel = AuthorityLevel.管理员,
            Description = "复活某个成员",
            Syntax = "[@qq号]",
            Tag = "群管理",
            SyntaxChecker = "At",
            IsPrivateAvailable = false,
            DailyLimit = 5)]
        public bool Reborn(MsgInformationEx MsgDTO, object[] param)
        {
            var aimQQ = (long) param[0];

            var cache = AliveStateMgr.GetState(MsgDTO.FromGroup, aimQQ);
            if (cache == null)
            {
                MsgSender.PushMsg(MsgDTO, "该成员不需要复活！", true);
                return false;
            }

            if (!WaiterSvc.WaitForConfirm_Gold(MsgDTO, 100))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            osPerson.Golds -= 100;
            osPerson.Update();

            cache.RebornTime = DateTime.Now;
            AliveStateMgr.Cache(cache);

            MsgSender.PushMsg(MsgDTO, $"复活成功！你当前剩余金币：{osPerson.Golds}", true);
            return true;
        }

        [EnterCommand(ID = "GroupManagerOnlyAI_AutoPowerOn",
            Command = "定时开机",
            AuthorityLevel = AuthorityLevel.管理员,
            Description = "定时整点开机",
            Syntax = "[整点数(0-23)]",
            Tag = "群管理",
            SyntaxChecker = "Long",
            IsPrivateAvailable = false)]
        public bool AutoPowerOn(MsgInformationEx MsgDTO, object[] param)
        {
            var hour = (int) (long) param[0];
            if (hour < 0 || hour > 23)
            {
                MsgSender.PushMsg(MsgDTO, "时间错误！");
                return false;
            }

            var setting = MongoService<AutoPowerSetting>.GetOnly(p => p.GroupNum == MsgDTO.FromGroup && p.Hour == hour);
            if (setting == null)
            {
                setting = new AutoPowerSetting(){GroupNum = MsgDTO.FromGroup, Hour = hour, ActionType = AutoPowerSettingActionType.PowerOn};
                MongoService<AutoPowerSetting>.Insert(setting);
            }
            else
            {
                setting.ActionType = AutoPowerSettingActionType.PowerOn;
                MongoService<AutoPowerSetting>.Update(setting);
            }

            MsgSender.PushMsg(MsgDTO, "设置成功！");
            return true;
        }

        [EnterCommand(ID = "GroupManagerOnlyAI_AutoPowerOff",
            Command = "定时关机",
            AuthorityLevel = AuthorityLevel.管理员,
            Description = "定时整点关机",
            Syntax = "[整点数(0-23)]",
            Tag = "群管理",
            SyntaxChecker = "Long",
            IsPrivateAvailable = false)]
        public bool AutoPowerOff(MsgInformationEx MsgDTO, object[] param)
        {
            var hour = (int) (long) param[0];
            if (hour < 0 || hour > 23)
            {
                MsgSender.PushMsg(MsgDTO, "时间错误！");
                return false;
            }

            var setting = MongoService<AutoPowerSetting>.GetOnly(p => p.GroupNum == MsgDTO.FromGroup && p.Hour == hour);
            if (setting == null)
            {
                setting = new AutoPowerSetting(){GroupNum = MsgDTO.FromGroup, Hour = hour, ActionType = AutoPowerSettingActionType.PowerOff};
                MongoService<AutoPowerSetting>.Insert(setting);
            }
            else
            {
                setting.ActionType = AutoPowerSettingActionType.PowerOff;
                MongoService<AutoPowerSetting>.Update(setting);
            }

            MsgSender.PushMsg(MsgDTO, "设置成功！");
            return true;
        }

        [EnterCommand(ID = "GroupManagerOnlyAI_ClearAutoPower",
            Command = "清除定时开关机 清除自动开关机",
            AuthorityLevel = AuthorityLevel.管理员,
            Description = "清除所有定时开关机设定",
            Syntax = "",
            Tag = "群管理",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public bool ClearAutoPower(MsgInformationEx MsgDTO, object[] param)
        {
            MongoService<AutoPowerSetting>.DeleteMany(p => p.GroupNum == MsgDTO.FromGroup);

            MsgSender.PushMsg(MsgDTO, "清除成功！");
            return true;
        }

        private bool SkillCheck(MsgInformationEx MsgDTO, long aimQQ, string key)
        {
            if (MsgDTO.FromQQ == aimQQ)
            {
                MsgSender.PushMsg(MsgDTO, "你无法对自己使用该能力！", true);
                return false;
            }

            var cache = AliveStateMgr.GetState(MsgDTO.FromGroup, MsgDTO.FromQQ);
            if (cache == null)
            {
                return true;
            }

            MsgSender.PushMsg(MsgDTO, $"他已经死了，你不能对尸体使用该能力！请于 {cache.RebornTime.ToString(CultureInfo.CurrentCulture)} 后再试！", true);
            return false;
        }

        private void DoSkill(MsgInformationEx MsgDTO, long aimQQ, string key, DateTime rebornTime, string skillName)
        {
            var cache = new AliveStateCache
            {
                QQNum = aimQQ,
                GroupNum = MsgDTO.FromGroup,
                Name = skillName,
                RebornTime = rebornTime
            };
            AliveStateMgr.Cache(cache);

            MsgSender.PushMsg(MsgDTO, $"成功对 {CodeApi.Code_At(aimQQ)} 使用了 {skillName}！他将于 {rebornTime.ToString(CultureInfo.CurrentCulture)} 后复活！");
        }
    }
}
