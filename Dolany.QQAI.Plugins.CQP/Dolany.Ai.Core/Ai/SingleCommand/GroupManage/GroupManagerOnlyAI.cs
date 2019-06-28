using System;
using System.Globalization;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.OnlineStore;
using Dolany.Database.Sqlite;

namespace Dolany.Ai.Core.Ai.SingleCommand.GroupManage
{
    [AI(Name = "死亡",
        Description = "AI for killing somebody for some time.",
        Enable = true,
        PriorityLevel = 16)]
    public class GroupManagerOnlyAI : AIBase
    {
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

            var key = $"AliveState-{MsgDTO.FromGroup}-{aimQQ}";
            var cache = SCacheService.Get<AliveStateCache>(key);
            if (cache == null)
            {
                MsgSender.PushMsg(MsgDTO, "该成员不需要复活！", true);
                return false;
            }

            if (!Waiter.Instance.WaitForConfirm_Gold(MsgDTO, 100))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            osPerson.Golds -= 100;
            osPerson.Update();

            SCacheService.Cache(key, cache, DateTime.Now);

            MsgSender.PushMsg(MsgDTO, $"复活成功！你当前剩余金币：{osPerson.Golds}", true);
            return true;
        }

        private static bool SkillCheck(MsgInformationEx MsgDTO, long aimQQ, string key)
        {
            if (MsgDTO.FromQQ == aimQQ)
            {
                MsgSender.PushMsg(MsgDTO, "你无法对自己使用该能力！", true);
                return false;
            }

            var cache = SCacheService.Get<AliveStateCache>(key);
            if (cache == null)
            {
                return true;
            }

            MsgSender.PushMsg(MsgDTO, $"他已经死了，你不能对尸体使用该能力！请于 {cache.RebornTime.ToString(CultureInfo.CurrentCulture)} 后再试！", true);
            return false;
        }

        private static void DoSkill(MsgInformationEx MsgDTO, long aimQQ, string key, DateTime rebornTime, string skillName)
        {
            var cache = new AliveStateCache
            {
                QQNum = aimQQ,
                GroupNum = MsgDTO.FromGroup,
                Name = skillName,
                RebornTime = rebornTime
            };
            SCacheService.Cache(key, cache, rebornTime);

            MsgSender.PushMsg(MsgDTO, $"成功对 {CodeApi.Code_At(aimQQ)} 使用了 {skillName}！他将于 {rebornTime.ToString(CultureInfo.CurrentCulture)} 后复活！");
        }
    }
}
