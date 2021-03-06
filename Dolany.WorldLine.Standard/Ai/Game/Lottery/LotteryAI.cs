﻿using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.UtilityTool;
using Dolany.WorldLine.Standard.OnlineStore;

namespace Dolany.WorldLine.Standard.Ai.Game.Lottery
{
    /// <summary>
    /// 抽奖模块
    /// </summary>
    public class LotteryAI : AIBase
    {
        public override string AIName { get; set; } = "开箱子";

        public override string Description { get; set; } = "AI for draw a lottery.";

        protected override CmdTagEnum DefaultTag { get; } = CmdTagEnum.商店功能;

        public LotterySvc LotterySvc { get; set; }
        public HonorSvc HonorSvc { get; set; }

        [EnterCommand(ID = "LotteryAI_DrawLottery",
            Command = "开箱子",
            Description = "花费100金币开一个箱子",
            IsPrivateAvailable = true,
            DailyLimit = 1,
            TestingDailyLimit = 3)]
        public bool DrawLottery(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (osPerson.Golds < LotterySvc.LotteryFee)
            {
                MsgSender.PushMsg(MsgDTO, $"你没有足够的金币开箱子({osPerson.Golds.CurencyFormat()}/{LotterySvc.LotteryFee.CurencyFormat()})", true);
                return false;
            }

            RandomLottery(MsgDTO);

            return true;
        }

        private void RandomLottery(MsgInformationEx MsgDTO)
        {
            var lottery = LotterySvc.RandLottery();

            var absBonus = lottery.Bonus - LotterySvc.LotteryFee;
            LotteryRecord.Record(absBonus);

            var personRec = LotteryPersonRecord.Get(MsgDTO.FromQQ);
            personRec.AddLottery(lottery.Name);
            personRec.Update();

            var msg = lottery.ToString();

            var golds = OSPerson.GoldConsume(MsgDTO.FromQQ, LotterySvc.LotteryFee - lottery.Bonus);
            msg += $"\r\n你当前持有金币：{golds.CurencyFormat()}";
            MsgSender.PushMsg(MsgDTO, msg, true);
        }

        [EnterCommand(ID = "LotteryAI_LimitBonus",
            Command = "抽奖",
            Description = "抽取一件随机当月限定物品",
            IsPrivateAvailable = true)]
        public bool LimitBonus(MsgInformationEx MsgDTO, object[] param)
        {
            var cache = PersonCacheRecord.Get(MsgDTO.FromQQ, "抽奖");
            if (string.IsNullOrEmpty(cache.Value) || !int.TryParse(cache.Value, out var times) || times <= 0)
            {
                MsgSender.PushMsg(MsgDTO, "你没有抽奖机会！", true);
                return false;
            }

            var items = HonorSvc.CurMonthLimitItems();
            var item  = items.ToDictionary(p => p, p => p.Rate).RandRated();

            var session = new MsgSession(MsgDTO);
            session.Add($"恭喜你抽到了 【{item.Name}】*1");
            session.Add($"    {item.Description} ");

            var record = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var m = record.ItemIncome(item.Name);
            if (!string.IsNullOrEmpty(m))
            {
                session.Add(m);
            }

            session.Send();

            cache.Value = (times - 1).ToString();
            cache.Update();

            return true;
        }
        
        [EnterCommand(ID                 = "LotteryAI_TenLimitBonus",
                      Command            = "十连抽",
                      Description        = "抽取十件随机当月限定物品",
                      IsPrivateAvailable = true)]
        public bool TenLimitBonus(MsgInformationEx MsgDTO, object[] param)
        {
            var cache = PersonCacheRecord.Get(MsgDTO.FromQQ, "抽奖");
            if (string.IsNullOrEmpty(cache.Value) || !int.TryParse(cache.Value, out var times) || times < 10)
            {
                MsgSender.PushMsg(MsgDTO, "你没有足够的抽奖机会！", true);
                return false;
            }

            var itemsRateDic     = HonorSvc.CurMonthLimitItems().ToDictionary(p => p, p => p.Rate);
            var randItems = SafeDictionary<string, int>.Empty;
            for (var i = 0; i < 10; i++)
            {
                var randItem = itemsRateDic.RandRated();
                randItems[randItem.Name] += 1;
            }
            
            var itemColle = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            foreach (var (name, count) in randItems.Data)
            {
                itemColle.ItemIncome(name, count);
            }

            var msg = $"恭喜你抽到了 {randItems.Data.Select(p => $"【{p.Key}】*{p.Value}").JoinToString(",")} ！";
            MsgSender.PushMsg(MsgDTO, msg, true);

            cache.Value = (times - 10).ToString();
            cache.Update();

            return true;
        }

        /// <summary>
        /// 兑换抽奖机会
        /// </summary>
        /// <param name="MsgDTO"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [EnterCommand(ID = "LotteryAI_GoldLimitBonus",
            Command = "兑换抽奖机会",
            Description = "用金币兑换抽奖机会",
            IsPrivateAvailable = true,
            DailyLimit = 3,
            TestingDailyLimit = 3)]
        public bool GoldLimitBonus(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (osPerson.Golds < 500)
            {
                MsgSender.PushMsg(MsgDTO, $"你没有足够的金币兑换（{osPerson.Golds.CurencyFormat()}/{500.CurencyFormat()}）", true);
                return false;
            }

            if (!WaiterSvc.WaitForConfirm_Gold(MsgDTO, 500))
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

            MsgSender.PushMsg(MsgDTO, $"兑换成功，你现在共有{times}次抽奖机会，快使用 【抽奖】 命令试试看吧！", true);

            return true;
        }

        /// <summary>
        /// 今日开箱统计
        /// </summary>
        /// <param name="MsgDTO"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [EnterCommand(ID = "LotteryAI_LotteryAnalyzeToday",
            Command = "今日开箱统计",
            Description = "获取今日开箱统计情况",
            IsPrivateAvailable = true)]
        public bool LotteryAnalyzeToday(MsgInformationEx MsgDTO, object[] param)
        {
            var todayRec = LotteryRecord.GetToday();
            MsgSender.PushMsg(MsgDTO, todayRec.ToString());

            return true;
        }

        /// <summary>
        /// 昨日开箱统计
        /// </summary>
        /// <param name="MsgDTO"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [EnterCommand(ID = "LotteryAI_LotteryAnalyzeYesterday",
            Command = "昨日开箱统计",
            Description = "获取昨日开箱统计情况",
            IsPrivateAvailable = true)]
        public bool LotteryAnalyzeYesterday(MsgInformationEx MsgDTO, object[] param)
        {
            var yesterdayRec = LotteryRecord.GetYesterday();
            MsgSender.PushMsg(MsgDTO, yesterdayRec.ToString());

            return true;
        }

        /// <summary>
        /// 我的开箱记录
        /// </summary>
        /// <param name="MsgDTO"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [EnterCommand(ID = "LotteryAI_MyLotteryRecord",
            Command = "我的开箱记录",
            Description = "获取自己的开箱统计",
            IsPrivateAvailable = true)]
        public bool MyLotteryRecord(MsgInformationEx MsgDTO, object[] param)
        {
            var rec = LotteryPersonRecord.Get(MsgDTO.FromQQ);
            if (rec.LotteryDic.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "你没有过任何开箱记录");
                return false;
            }

            MsgSender.PushMsg(MsgDTO, rec.ToString());

            return true;
        }
    }
}
