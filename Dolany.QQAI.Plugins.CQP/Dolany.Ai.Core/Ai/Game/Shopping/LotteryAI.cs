using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.API;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.OnlineStore;

namespace Dolany.Ai.Core.Ai.Game.Shopping
{
    public class LotteryAI : AIBase
    {
        public override string AIName { get; set; } = "彩票";

        public override string Description { get; set; } = "AI for draw a lottery.";

        public override int PriorityLevel { get; set; } = 10;

        private const int LotteryFee = 100;

        private Dictionary<int, int> LotteryDic;

        private int SumRate;

        public override bool NeedManualOpeon { get; set; } = true;

        public override void Initialization()
        {
            base.Initialization();

            LotteryDic = CommonUtil.ReadJsonData<Dictionary<int, int>>("LotteryData");
            SumRate = LotteryDic.Values.Sum();
        }

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
            if (osPerson.Golds < LotteryFee)
            {
                MsgSender.PushMsg(MsgDTO, $"你没有足够的金币购买彩票({osPerson.Golds}/{LotteryFee})", true);
                return false;
            }

            RandomLottery(MsgDTO);

            return true;
        }

        private void RandomLottery(MsgInformationEx MsgDTO)
        {
            var index = CommonUtil.RandInt(SumRate);

            var totalSum = 0;
            var bonus = 0;
            foreach (var (key, value) in LotteryDic)
            {
                if (index < totalSum + value)
                {
                    bonus = key;
                    break;
                }

                totalSum += value;
            }

            var absBonus = bonus - LotteryFee;
            var msg = absBonus > 0 ? $"恭喜你赚得了 {absBonus}{Emoji.钱袋}" : $"很遗憾你损失了 {absBonus}{Emoji.钱袋}";
            msg += $"(已扣除成本费{LotteryFee}{Emoji.钱袋})";

            var golds = OSPerson.GoldConsume(MsgDTO.FromQQ, LotteryFee - bonus);
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
    }
}
