using System;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.Model;
using Dolany.Ai.Core.OnlineStore;
using Dolany.Database.Sqlite;

namespace Dolany.Ai.Core.Ai.Game.Shopping
{
    [AI(
        Name = "签到",
        Description = "AI for Everyday Signing In.",
        Enable = true,
        PriorityLevel = 10,
        NeedManulOpen = true)]
    public class SignInAI : AIBase
    {
        [EnterCommand(Command = "签到",
            AuthorityLevel = AuthorityLevel.管理员,
            Description = "设置签到内容，有效期1个月(不能与系统自带命令重复)",
            Syntax = "[签到内容]",
            Tag = "商店功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = false)]
        public bool SetSignContent(MsgInformationEx MsgDTO, object[] param)
        {
            var content = param[0] as string;

            if (AIMgr.Instance.AllAvailableGroupCommands.Any(comm => comm.Command == content))
            {
                MsgSender.PushMsg(MsgDTO, "不能与系统自带命令重复！");
                return false;
            }

            SCacheService.Cache($"DailySignIn-{MsgDTO.FromGroup}", content, DateTime.Today.AddMonths(1));
            MsgSender.PushMsg(MsgDTO, "设置成功！");
            return true;
        }

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            if (MsgDTO.Type == MsgType.Private || !GroupSettingMgr.Instance[MsgDTO.FromGroup].HasFunction(Attr.Name))
            {
                return false;
            }

            // 群组签到设置
            var cache = SCacheService.Get<string>($"DailySignIn-{MsgDTO.FromGroup}");
            if (string.IsNullOrEmpty(cache) && MsgDTO.FullMsg != "签到")
            {
                return false;
            }

            if (!string.IsNullOrEmpty(cache) && MsgDTO.FullMsg != cache)
            {
                return false;
            }

            AIAnalyzer.AddCommandCount(new CommandAnalyzeDTO()
            {
                Ai = Attr.Name,
                Command = "SignInOverride",
                GroupNum = MsgDTO.FromGroup
            });
            // 个人签到记录
            var signCache = SCacheService.Get<DailySignInCache>($"DailySignIn-{MsgDTO.FromGroup}-{MsgDTO.FromQQ}");
            if (signCache != null && signCache.LastSignDate.ToLocalTime() == DateTime.Today)
            {
                MsgSender.PushMsg(MsgDTO, "你今天已经签到过了！", true);
                return false;
            }

            signCache = Sign(MsgDTO, signCache);
            SCacheService.Cache($"DailySignIn-{MsgDTO.FromGroup}-{MsgDTO.FromQQ}", signCache, CommonUtil.UntilTommorow().AddDays(1));
            return true;
        }

        private DailySignInCache Sign(MsgInformationEx MsgDTO, DailySignInCache signCache)
        {
            if (signCache == null)
            {
                signCache = new DailySignInCache()
                {
                    GroupNum = MsgDTO.FromGroup,
                    QQNum = MsgDTO.FromQQ
                };
            }

            var key = "SignInAcc";
            if (string.IsNullOrEmpty(SCacheService.Get<string>(key)))
            {
                signCache.SuccessiveSignDays++;
            }
            else
            {
                signCache.SuccessiveSignDays += 2;
            }

            signCache.LastSignDate = DateTime.Today;
            var goldsGen = signCache.SuccessiveSignDays > 5 ? 25 : signCache.SuccessiveSignDays * 5;

            OSPerson.GoldIncome(MsgDTO.FromQQ, goldsGen);

            var msg = $"签到成功！你已连续签到 {signCache.SuccessiveSignDays}天，获得 {goldsGen}金币！";
            MsgSender.PushMsg(MsgDTO, msg, true);

            return signCache;
        }

        [EnterCommand(Command = "今日签到内容",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取今日签到内容",
            Syntax = "",
            Tag = "商店功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public bool TodaySignContent(MsgInformationEx MsgDTO, object[] param)
        {
            var cache = SCacheService.Get<string>($"DailySignIn-{MsgDTO.FromGroup}");
            MsgSender.PushMsg(MsgDTO, $"今日签到内容是：{(string.IsNullOrEmpty(cache) ? "签到" : cache)}");
            return true;
        }
    }

    public class DailySignInCache
    {
        public long GroupNum { get; set; }

        public long QQNum { get; set; }

        public int SuccessiveSignDays { get; set; }

        public DateTime LastSignDate { get; set; }
    }
}
