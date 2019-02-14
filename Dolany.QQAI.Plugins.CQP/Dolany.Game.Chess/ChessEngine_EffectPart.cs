using System;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;
using Dolany.Database.Ai;
using Dolany.Database.Sqlite;
using Dolany.Database.Sqlite.Model;
using Dolany.Game.OnlineStore;

namespace Dolany.Game.Chess
{
    public partial class ChessEngine
    {
        [ChessEffect(Name = "昙天",
            Description = "36小时内不可以捞瓶子")]
        public void 昙天()
        {
            OSPerson.AddBuff(SelfQQNum, new OSPersonBuff
            {
                Name = "昙天",
                Description = "36小时内不可以捞瓶子",
                ExpiryTime = DateTime.Now.AddHours(36),
                IsPositive = false
            });
        }

        [ChessEffect(Name = "浓雾",
            Description = "随机复制一件对手的物品")]
        public void 浓雾()
        {
            var query = MongoService<DriftItemRecord>.Get(r => r.QQNum == AimQQNum).FirstOrDefault();
            if (query == null || query.ItemCount.IsNullOrEmpty())
            {
                MsgCallBack("对手没有任何物品！", GroupNum, SelfQQNum);
                return;
            }

            var item = query.ItemCount[CommonUtil.RandInt(query.ItemCount.Count)];
            var (msg, _) = ItemHelper.Instance.ItemIncome(SelfQQNum, item.Name);
            MsgCallBack($"你获得了 {item.Name}！\r{msg}", GroupNum, SelfQQNum);
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
            Description = "24小时内商店购买享有40%的折扣")]
        public void 极光()
        {
            OSPerson.AddBuff(SelfQQNum, new OSPersonBuff
            {
                Name = "极光",
                Description = "24小时内商店购买享有40%的折扣",
                ExpiryTime = DateTime.Now.AddHours(24),
                IsPositive = true
            });
        }

        [ChessEffect(Name = "黄砂",
            Description = "48小时内无法再次挑战")]
        public void 黄砂()
        {
            OSPerson.AddBuff(SelfQQNum, new OSPersonBuff
            {
                Name = "黄砂",
                Description = "48小时内无法再次挑战",
                ExpiryTime = DateTime.Now.AddHours(48),
                IsPositive = false
            });
        }

        [ChessEffect(Name = "梅雨",
            Description = "48小时内捞瓶子成功率下降30%")]
        public void 梅雨()
        {
            OSPerson.AddBuff(SelfQQNum, new OSPersonBuff
            {
                Name = "梅雨",
                Description = "48小时内捞瓶子成功率下降30%",
                ExpiryTime = DateTime.Now.AddHours(48),
                IsPositive = false
            });
        }

        [ChessEffect(Name = "快晴",
            Description = "36小时内无法从事商业活动（贩卖/购买/交易）")]
        public void 快晴()
        {
            OSPerson.AddBuff(SelfQQNum, new OSPersonBuff
            {
                Name = "快晴",
                Description = "36小时内无法从事商业活动（贩卖/购买/交易）",
                ExpiryTime = DateTime.Now.AddHours(36),
                IsPositive = false
            });
        }

        [ChessEffect(Name = "雾雨",
            Description = "随机获得一个商店正在出售的物品")]
        public void 雾雨()
        {
            var sellingItems = TransHelper.GetDailySellItems();
            var item = sellingItems[CommonUtil.RandInt(sellingItems.Length)];
            var msg = ItemHelper.Instance.ItemIncome(SelfQQNum, item.Name);

            MsgCallBack($"你获得了：{item.Name}\r{msg}", GroupNum, SelfQQNum);
        }

        [ChessEffect(Name = "苍天",
            Description = "24小时内进行交易时享有40%的折扣")]
        public void 苍天()
        {
            OSPerson.AddBuff(SelfQQNum, new OSPersonBuff
            {
                Name = "苍天",
                Description = "24小时内进行交易时享有40%的折扣",
                ExpiryTime = DateTime.Now.AddHours(24),
                IsPositive = true
            });
        }

        [ChessEffect(Name = "雹",
            Description = "强制贩卖一个随机物品给系统商店")]
        public void 雹()
        {
            var record = MongoService<DriftItemRecord>.Get(r => r.QQNum == SelfQQNum).FirstOrDefault();
            if (record == null || record.ItemCount.IsNullOrEmpty())
            {
                MsgCallBack("你没有任何物品", GroupNum, SelfQQNum);
                return;
            }

            var randIdx = CommonUtil.RandInt(record.ItemCount.Count);
            var item = record.ItemCount[randIdx];

            var golds = TransHelper.SellItemToShop(SelfQQNum, item.Name);
            MsgCallBack($"你贩卖了 {item.Name}\r你当前拥有金币 {golds}", GroupNum, SelfQQNum);
        }

        [ChessEffect(Name = "花昙",
            Description = "随机清除自身的一个负面状态")]
        public void 花昙()
        {
            var selfOs = OSPerson.GetPerson(SelfQQNum);
            var effectiveBuffs = selfOs.Buffs?.Where(b => b.ExpiryTime.ToLocalTime() > DateTime.Now).ToArray();
            var posBuffCount = effectiveBuffs?.Count(b => !b.IsPositive) ?? 0;
            if (posBuffCount == 0)
            {
                MsgCallBack("你没有任何负面状态", GroupNum, SelfQQNum);
                return;
            }

            var buff = effectiveBuffs?[CommonUtil.RandInt(posBuffCount)];
            selfOs.Buffs?.Remove(buff);
            MongoService<OSPerson>.Update(selfOs);

            MsgCallBack($"移除了 {buff?.Name}:{buff?.Description}", GroupNum, SelfQQNum);
        }

        [ChessEffect(Name = "天气雨",
            Description = "随机移除对方的一个增益状态")]
        public void 天气雨()
        {
            var oppeOs = OSPerson.GetPerson(AimQQNum);
            var effectiveBuffs = oppeOs.Buffs?.Where(b => b.ExpiryTime.ToLocalTime() > DateTime.Now).ToArray();
            var posBuffCount = effectiveBuffs?.Count(b => b.IsPositive) ?? 0;
            if (posBuffCount == 0)
            {
                MsgCallBack("对手没有增益状态", GroupNum, SelfQQNum);
                return;
            }

            var buff = effectiveBuffs?[CommonUtil.RandInt(posBuffCount)];
            oppeOs.Buffs?.Remove(buff);
            MongoService<OSPerson>.Update(oppeOs);

            MsgCallBack($"移除了 {buff?.Name}:{buff?.Description}", GroupNum, SelfQQNum);
        }

        [ChessEffect(Name = "疏雨",
            Description = "24小时内将物品贩卖给商店时将额外获得40%的金币")]
        public void 疏雨()
        {
            OSPerson.AddBuff(SelfQQNum, new OSPersonBuff
            {
                Name = "疏雨",
                Description = "24小时内将物品贩卖给商店时将额外获得40%的金币",
                ExpiryTime = DateTime.Now.AddHours(24),
                IsPositive = true
            });
        }

        [ChessEffect(Name = "川雾",
            Description = "随机复制对手一个负面状态到自己身上")]
        public void 川雾()
        {
            var oppeOs = OSPerson.GetPerson(AimQQNum);
            var effectiveBuffs = oppeOs.Buffs?.Where(b => b.ExpiryTime.ToLocalTime() > DateTime.Now).ToArray();
            var posBuffCount = effectiveBuffs?.Count(b => !b.IsPositive) ?? 0;
            if (posBuffCount == 0)
            {
                MsgCallBack("对手没有负面状态", GroupNum, SelfQQNum);
                return;
            }

            var buff = effectiveBuffs?[CommonUtil.RandInt(posBuffCount)];
            OSPerson.AddBuff(SelfQQNum, buff);

            MsgCallBack($"复制到了 {buff?.Name}:{buff?.Description}", GroupNum, SelfQQNum);
        }

        [ChessEffect(Name = "台风",
            Description = "随机复制对手一个增益状态到自己身上")]
        public void 台风()
        {
            var oppeOs = OSPerson.GetPerson(AimQQNum);
            var effectiveBuffs = oppeOs.Buffs?.Where(b => b.ExpiryTime.ToLocalTime() > DateTime.Now).ToArray();
            var posBuffCount = effectiveBuffs?.Count(b => b.IsPositive) ?? 0;
            if (posBuffCount == 0)
            {
                MsgCallBack("对手没有增益状态", GroupNum, SelfQQNum);
                return;
            }

            var buff = effectiveBuffs?[CommonUtil.RandInt(posBuffCount)];
            OSPerson.AddBuff(SelfQQNum, buff);

            MsgCallBack($"复制到了 {buff?.Name}:{buff?.Description}", GroupNum, SelfQQNum);
        }

        [ChessEffect(Name = "凪",
            Description = "增加一次捞瓶子的机会(当日有效)")]
        public void 凪()
        {
            var key = $"DailyLimit-捞瓶子-{SelfQQNum}";
            var cache = SCacheService.Get<DailyLimitCache>(key);
            if (cache == null)
            {
                SCacheService.Cache(key, new DailyLimitCache{QQNum = SelfQQNum, Count = -1, Command = "捞瓶子"});
            }
            else
            {
                cache.Count -= 1;
                SCacheService.Cache(key, cache);
            }
        }

        [ChessEffect(Name = "钻石尘",
            Description = "48小时内捞瓶子时有50%的概率丢失40金币")]
        public void 钻石尘()
        {
            OSPerson.AddBuff(SelfQQNum, new OSPersonBuff
            {
                Name = "钻石尘",
                Description = "48小时内捞瓶子时有50%的概率丢失40金币",
                ExpiryTime = DateTime.Now.AddHours(48),
                IsPositive = false
            });
        }
    }
}
