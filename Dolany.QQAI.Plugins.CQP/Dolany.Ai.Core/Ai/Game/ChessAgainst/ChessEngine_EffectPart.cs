using System;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.OnlineStore;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Ai.Game.ChessAgainst
{
    public partial class ChessEngine
    {
        [ChessEffect(Name = "昙天",
            Description = "12小时内不可以捞瓶子")]
        public void 昙天()
        {
            var osPerson = OSPerson.GetPerson(SelfQQNum);
            osPerson.AddBuff(new OSPersonBuff
            {
                Name = "昙天",
                Description = "不可以捞瓶子",
                ExpiryTime = DateTime.Now.AddHours(12),
                IsPositive = false,
                Data = 1,
                Source = AimQQNum
            });
            osPerson.Update();
        }

        [ChessEffect(Name = "浓雾",
            Description = "随机复制一件对手的物品")]
        public void 浓雾()
        {
            var query = DriftItemRecord.GetRecord(AimQQNum);
            if (query == null || query.ItemCount.IsNullOrEmpty())
            {
                MsgSender.PushMsg(GroupNum, 0, "对手没有任何物品！");
                return;
            }

            var item = query.ItemCount.RandElement();
            var record = DriftItemRecord.GetRecord(SelfQQNum);
            var msg = record.ItemIncome(item.Name);
            MsgSender.PushMsg(GroupNum, 0, $"你获得了 {item.Name}！\r{msg}");
        }

        [ChessEffect(Name = "烈日",
            Description = "抢夺对方50金币")]
        public void 烈日()
        {
            OSPerson.GoldConsume(AimQQNum, 50);
            OSPerson.GoldIncome(SelfQQNum, 50);
        }

        [ChessEffect(Name = "雪",
            Description = "丢失50金币")]
        public void 雪()
        {
            OSPerson.GoldConsume(SelfQQNum, 50);
        }

        [ChessEffect(Name = "晴岚",
            Description = "没有任何事情发生")]
        public void 晴岚()
        {

        }

        [ChessEffect(Name = "风雨",
            Description = "对方获得50金币")]
        public void 风雨()
        {
            OSPerson.GoldIncome(AimQQNum, 50);
        }

        [ChessEffect(Name = "极光",
            Description = "12小时内商店购买享有20%的折扣")]
        public void 极光()
        {
            var osPerson = OSPerson.GetPerson(SelfQQNum);
            osPerson.AddBuff(new OSPersonBuff
            {
                Name = "极光",
                Description = "商店购买享有20%的折扣",
                ExpiryTime = DateTime.Now.AddHours(12),
                IsPositive = true,
                Data = 1,
                Source = AimQQNum
            });
            osPerson.Update();
        }

        [ChessEffect(Name = "黄砂",
            Description = "12小时内无法再次挑战")]
        public void 黄砂()
        {
            var osPerson = OSPerson.GetPerson(SelfQQNum);
            osPerson.AddBuff(new OSPersonBuff
            {
                Name = "黄砂",
                Description = "无法再次挑战",
                ExpiryTime = DateTime.Now.AddHours(12),
                IsPositive = false,
                Data = 1,
                Source = AimQQNum
            });
            osPerson.Update();
        }

        [ChessEffect(Name = "梅雨",
            Description = "12小时内捞瓶子成功率下降30%")]
        public void 梅雨()
        {
            var osPerson = OSPerson.GetPerson(SelfQQNum);
            osPerson.AddBuff(new OSPersonBuff
            {
                Name = "梅雨",
                Description = "捞瓶子成功率下降30%",
                ExpiryTime = DateTime.Now.AddHours(12),
                IsPositive = false,
                Data = 1,
                Source = AimQQNum
            });
            osPerson.Update();
        }

        [ChessEffect(Name = "快晴",
            Description = "12小时内无法从事商业活动（贩卖/购买/交易）")]
        public void 快晴()
        {
            var osPerson = OSPerson.GetPerson(SelfQQNum);
            osPerson.AddBuff(new OSPersonBuff
            {
                Name = "快晴",
                Description = "无法从事商业活动（贩卖/购买/交易）",
                ExpiryTime = DateTime.Now.AddHours(12),
                IsPositive = false,
                Data = 1,
                Source = AimQQNum
            });
            osPerson.Update();
        }

        [ChessEffect(Name = "雾雨",
            Description = "随机获得一个商店正在出售的物品")]
        public void 雾雨()
        {
            var sellingItems = TransHelper.GetDailySellItems();
            var item = sellingItems.RandElement();
            var record = DriftItemRecord.GetRecord(SelfQQNum);
            var msg = record.ItemIncome(item.Name);

            MsgSender.PushMsg(GroupNum, 0, $"你获得了：{item.Name}\r{msg}");
        }

        [ChessEffect(Name = "苍天",
            Description = "12小时内进行交易时免除手续费")]
        public void 苍天()
        {
            var osPerson = OSPerson.GetPerson(SelfQQNum);
            osPerson.AddBuff(new OSPersonBuff
            {
                Name = "苍天",
                Description = "进行交易时免除手续费",
                ExpiryTime = DateTime.Now.AddHours(12),
                IsPositive = true,
                Data = 1,
                Source = AimQQNum
            });
            osPerson.Update();
        }

        [ChessEffect(Name = "雹",
            Description = "强制贩卖一个随机物品给系统商店")]
        public void 雹()
        {
            var record = DriftItemRecord.GetRecord(SelfQQNum);
            if (record.ItemCount.IsNullOrEmpty())
            {
                MsgSender.PushMsg(GroupNum, 0, "你没有任何物品");
                return;
            }

            var commonItems = record.ItemCount.Where(ic => !HonorHelper.Instance.IsLimit(ic.Name)).ToList();
            if (commonItems.IsNullOrEmpty())
            {
                MsgSender.PushMsg(GroupNum, 0, "你没有任何非限定物品");
                return;
            }

            var item = commonItems.RandElement();

            var golds = TransHelper.SellItemToShop(SelfQQNum, item.Name);
            MsgSender.PushMsg(GroupNum, 0, $"你贩卖了 {item.Name}\r你当前拥有金币 {golds}");
        }

        [ChessEffect(Name = "花昙",
            Description = "随机清除自身的一个负面状态")]
        public void 花昙()
        {
            var selfOs = OSPerson.GetPerson(SelfQQNum);
            var effectiveBuffs = selfOs.Buffs?.Where(b => b.ExpiryTime.ToLocalTime() > DateTime.Now && !b.IsPositive).ToArray();
            if (effectiveBuffs.IsNullOrEmpty())
            {
                MsgSender.PushMsg(GroupNum, 0, "你没有任何负面状态");
                return;
            }

            var buff = effectiveBuffs?.RandElement();
            selfOs.Buffs?.Remove(buff);
            selfOs.Update();

            MsgSender.PushMsg(GroupNum, 0, $"移除了 {buff?.Name}:{buff?.Description}");
        }

        [ChessEffect(Name = "天气雨",
            Description = "随机移除对方的一个增益状态")]
        public void 天气雨()
        {
            var oppeOs = OSPerson.GetPerson(AimQQNum);
            var effectiveBuffs = oppeOs.Buffs?.Where(b => b.ExpiryTime.ToLocalTime() > DateTime.Now && b.IsPositive).ToArray();
            if (effectiveBuffs.IsNullOrEmpty())
            {
                MsgSender.PushMsg(GroupNum, 0, "对手没有增益状态");
                return;
            }

            var buff = effectiveBuffs?.RandElement();
            oppeOs.Buffs?.Remove(buff);
            oppeOs.Update();

            MsgSender.PushMsg(GroupNum, 0, $"移除了 {buff?.Name}:{buff?.Description}");
        }

        [ChessEffect(Name = "疏雨",
            Description = "12小时内将物品贩卖给商店时将额外获得20%的金币")]
        public void 疏雨()
        {
            var osPerson = OSPerson.GetPerson(SelfQQNum);
            osPerson.AddBuff(new OSPersonBuff
            {
                Name = "疏雨",
                Description = "将物品贩卖给商店时将额外获得20%的金币",
                ExpiryTime = DateTime.Now.AddHours(12),
                IsPositive = true,
                Source = AimQQNum
            });
            osPerson.Update();
        }

        [ChessEffect(Name = "川雾",
            Description = "随机复制对手一个负面状态到自己身上")]
        public void 川雾()
        {
            var oppeOs = OSPerson.GetPerson(AimQQNum);
            var effectiveBuffs = oppeOs.Buffs?.Where(b => b.ExpiryTime.ToLocalTime() > DateTime.Now && !b.IsPositive).ToArray();
            if (effectiveBuffs.IsNullOrEmpty())
            {
                MsgSender.PushMsg(GroupNum, 0, "对手没有负面状态");
                return;
            }

            var buff = effectiveBuffs?.RandElement();
            var osPerson = OSPerson.GetPerson(SelfQQNum);
            osPerson.AddBuff(buff);
            osPerson.Update();

            MsgSender.PushMsg(GroupNum, 0, $"复制到了 {buff?.Name}:{buff?.Description}");
        }

        [ChessEffect(Name = "台风",
            Description = "随机复制对手一个增益状态到自己身上")]
        public void 台风()
        {
            var oppeOs = OSPerson.GetPerson(AimQQNum);
            var effectiveBuffs = oppeOs.Buffs?.Where(b => b.ExpiryTime.ToLocalTime() > DateTime.Now && b.IsPositive).ToArray();
            if (effectiveBuffs.IsNullOrEmpty())
            {
                MsgSender.PushMsg(GroupNum, 0, "对手没有增益状态");
                return;
            }

            var buff = effectiveBuffs?.RandElement();
            var osPerson = OSPerson.GetPerson(SelfQQNum);
            osPerson.AddBuff(buff);
            osPerson.Update();

            MsgSender.PushMsg(GroupNum, 0, $"复制到了 {buff?.Name}:{buff?.Description}");
        }

        [ChessEffect(Name = "凪",
            Description = "增加一次捞瓶子的机会(当日有效)")]
        public void 凪()
        {
            var dailyLimit = DailyLimitRecord.Get(SelfQQNum, "捞瓶子");
            dailyLimit.Decache();
            dailyLimit.Update();
        }

        [ChessEffect(Name = "钻石尘",
            Description = "12小时内捞瓶子时有50%的概率丢失40金币")]
        public void 钻石尘()
        {
            var osPerson = OSPerson.GetPerson(SelfQQNum);
            osPerson.AddBuff(new OSPersonBuff
            {
                Name = "钻石尘",
                Description = "捞瓶子时有50%的概率丢失40金币",
                ExpiryTime = DateTime.Now.AddHours(12),
                IsPositive = false,
                Source = AimQQNum
            });
            osPerson.Update();
        }
    }
}
