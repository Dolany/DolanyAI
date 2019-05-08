using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Model;
using Dolany.Ai.Core.OnlineStore;
using Dolany.Database.Ai;
using Dolany.Database.Sqlite;

namespace Dolany.Ai.Core.Ai.Game.Shopping
{
    [AI(
        Name = "彩票",
        Description = "AI for draw a lottery.",
        Enable = true,
        PriorityLevel = 10,
        NeedManulOpen = true)]
    public class LotteryAI : AIBase
    {
        private const int LotteryFee = 100;

        private Dictionary<int, int> LotteryDic;

        private int SumRate;

        public override void Initialization()
        {
            base.Initialization();

            LotteryDic = CommonUtil.ReadJsonData<Dictionary<int, int>>("LotteryData");
            SumRate = LotteryDic.Values.Sum();
        }

        [EnterCommand(
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
                MsgSender.PushMsg(MsgDTO, "你没有足够的金币购买彩票！", true);
                return false;
            }

            if (!Waiter.Instance.WaitForConfirm_Gold(MsgDTO, LotteryFee, 7))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            var golds = RandomLottery(MsgDTO);

            Thread.Sleep(1000);
            MsgSender.PushMsg(MsgDTO, $"你当前持有金币：{golds}", true);
            return true;
        }

        private int RandomLottery(MsgInformationEx MsgDTO)
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

            MsgSender.PushMsg(MsgDTO, bonus == 0 ? "谢谢参与！" : $"恭喜你中奖啦！奖金：{bonus}", true);

            var golds = OSPerson.GoldConsume(MsgDTO.FromQQ, LotteryFee - bonus);
            return golds;
        }

        [EnterCommand(
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
                MsgSender.PushMsg(MsgDTO, "你没有抽奖机会！");
                return false;
            }

            var items = HonorHelper.Instance.CurMonthLimitItems();
            var item = items.RandElement();

            var msg = $"恭喜你抽到了 {item.Name}*1";
            var record = DriftItemRecord.GetRecord(MsgDTO.FromQQ);
            var m = record.ItemIncome(item.Name);
            if (!string.IsNullOrEmpty(m))
            {
                msg += '\r' + m;
            }

            MsgSender.PushMsg(MsgDTO, msg, true);

            cache.Value = (times - 1).ToString();

            return true;
        }
    }
}
