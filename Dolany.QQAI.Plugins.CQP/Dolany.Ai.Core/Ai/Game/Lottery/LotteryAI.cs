using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.API;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.OnlineStore;

namespace Dolany.Ai.Core.Ai.Game.Lottery
{
    public class LotteryAI : AIBase
    {
        public override string AIName { get; set; } = "彩票";

        public override string Description { get; set; } = "AI for draw a lottery.";

        public override int PriorityLevel { get; set; } = 10;

        public override bool NeedManualOpeon { get; set; } = true;

        [EnterCommand(ID = "LotteryAI_DrawLottery",
            Command = "买彩票",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "买一张彩票获得随机效果",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "商店功能",
            IsPrivateAvailable = true,
            DailyLimit = 1,
            TestingDailyLimit = 3)]
        public bool DrawLottery(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (osPerson.Golds < LotteryMgr.LotteryFee)
            {
                MsgSender.PushMsg(MsgDTO, $"你没有足够的金币购买彩票({osPerson.Golds}/{LotteryMgr.LotteryFee})", true);
                return false;
            }

            RandomLottery(MsgDTO);

            return true;
        }

        private static void RandomLottery(MsgInformationEx MsgDTO)
        {
            var lottery = LotteryMgr.Instance.RandLottery();

            var absBonus = lottery.Bonus - LotteryMgr.LotteryFee;
            LotteryRecord.Record(absBonus);

            var personRec = LotteryPersonRecord.Get(MsgDTO.FromQQ);
            personRec.AddLottery(lottery.Name);
            personRec.Update();

            var msg = lottery.ToString();

            var golds = OSPerson.GoldConsume(MsgDTO.FromQQ, LotteryMgr.LotteryFee - lottery.Bonus);
            msg += $"\r你当前持有金币：{golds}{Emoji.钱袋}";
            MsgSender.PushMsg(MsgDTO, msg, true);
        }

        [EnterCommand(ID = "LotteryAI_LimitBonus",
            Command = "抽奖",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "抽取一件随机当月限定物品",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "商店功能",
            IsPrivateAvailable = true)]
        public bool LimitBonus(MsgInformationEx MsgDTO, object[] param)
        {
            var cache = PersonCacheRecord.Get(MsgDTO.FromQQ, "抽奖");
            if (string.IsNullOrEmpty(cache.Value) || !int.TryParse(cache.Value, out var times) || times <= 0)
            {
                MsgSender.PushMsg(MsgDTO, "你没有抽奖机会！", true);
                return false;
            }

            var items = HonorHelper.Instance.CurMonthLimitItems();
            var item = items.RandElement();

            var msg = $"恭喜你抽到了 {item.Name}*1\r" +
                      $"    {item.Description} ";
            var record = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var m = record.ItemIncome(item.Name);
            if (!string.IsNullOrEmpty(m))
            {
                msg += '\r' + m;
            }

            MsgSender.PushMsg(MsgDTO, msg, true);

            cache.Value = (times - 1).ToString();
            cache.Update();

            return true;
        }

        [EnterCommand(ID = "LotteryAI_GoldLimitBonus",
            Command = "兑换抽奖机会",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "用金币兑换抽奖机会",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "商店功能",
            IsPrivateAvailable = true,
            DailyLimit = 3,
            TestingDailyLimit = 3)]
        public bool GoldLimitBonus(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (osPerson.Golds < 500)
            {
                MsgSender.PushMsg(MsgDTO, $"你没有足够的金币兑换（{osPerson.Golds}/500）", true);
                return false;
            }

            if (!Waiter.Instance.WaitForConfirm_Gold(MsgDTO, 500))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消");
                return false;
            }

            osPerson.Golds -= 500;

            var cache = PersonCacheRecord.Get(MsgDTO.FromQQ, "抽奖");
            if (!int.TryParse(cache.Value, out var times))
            {
                times = 0;
            }

            times++;
            cache.Value = times.ToString();

            osPerson.Update();
            cache.Update();

            MsgSender.PushMsg(MsgDTO, $"兑换成功，你现在共有{times}次抽奖机会，快使用 抽奖 命令试试看吧！", true);

            return true;
        }

        [EnterCommand(ID = "LotteryAI_LotteryAnalyzeToday",
            Command = "今日彩票统计",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取今日彩票统计情况",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "商店功能",
            IsPrivateAvailable = true)]
        public bool LotteryAnalyzeToday(MsgInformationEx MsgDTO, object[] param)
        {
            var todayRec = LotteryRecord.GetToday();
            MsgSender.PushMsg(MsgDTO, todayRec.ToString());

            return true;
        }

        [EnterCommand(ID = "LotteryAI_LotteryAnalyzeYesterday",
            Command = "昨日彩票统计",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取昨日彩票统计情况",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "商店功能",
            IsPrivateAvailable = true)]
        public bool LotteryAnalyzeYesterday(MsgInformationEx MsgDTO, object[] param)
        {
            var yesterdayRec = LotteryRecord.GetYesterday();
            MsgSender.PushMsg(MsgDTO, yesterdayRec.ToString());

            return true;
        }

        [EnterCommand(ID = "LotteryAI_MyLotteryRecord",
            Command = "我的彩票记录",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取自己的彩票统计",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "商店功能",
            IsPrivateAvailable = true)]
        public bool MyLotteryRecord(MsgInformationEx MsgDTO, object[] param)
        {
            var rec = LotteryPersonRecord.Get(MsgDTO.FromQQ);
            if (rec.LotteryDic.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "你没有过任何彩票记录");
                return false;
            }

            MsgSender.PushMsg(MsgDTO, rec.ToString());

            return true;
        }
    }
}
