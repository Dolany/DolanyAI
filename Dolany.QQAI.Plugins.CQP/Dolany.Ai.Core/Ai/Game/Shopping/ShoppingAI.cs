using System;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Ai.Game.Advanture;
using Dolany.Ai.Core.Ai.Game.Gift;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.Model;
using Dolany.Ai.Core.OnlineStore;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Ai.Game.Shopping
{
    [AI(Name = "商店",
        Description = "AI for Shopping.",
        Enable = true,
        PriorityLevel = 10)]
    public class ShoppingAI : AIBase
    {
        [EnterCommand(ID = "ShoppingAI_Sell",
            Command = "贩卖 出售",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "贩卖物品或者成就",
            Syntax = "[物品名或成就名]",
            Tag = "商店功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true,
            DailyLimit = 6,
            TestingDailyLimit = 8)]
        public bool Sell(MsgInformationEx MsgDTO, object[] param)
        {
            if (OSPersonBuff.CheckBuff(MsgDTO.FromQQ, "快晴"))
            {
                MsgSender.PushMsg(MsgDTO, "你无法进行该操作！(快晴)");
                return false;
            }

            var name = param[0] as string;

            var item = HonorHelper.Instance.FindItem(name);
            if (item != null)
            {
                SellItem(MsgDTO, item);
                return true;
            }

            if (HonorHelper.Instance.FindHonor(name) != null)
            {
                SellHonor(MsgDTO, name);
                return true;
            }

            MsgSender.PushMsg(MsgDTO, "未查找到相关物品或成就！");
            return false;
        }

        [EnterCommand(ID = "ShoppingAI_SellRedundant",
            Command = "贩卖多余物品 出售多余物品",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "一键贩卖自己多余的物品",
            Syntax = "",
            Tag = "商店功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true,
            DailyLimit = 2,
            TestingDailyLimit = 2)]
        public bool SellRedundant(MsgInformationEx MsgDTO, object[] param)
        {
            if (OSPersonBuff.CheckBuff(MsgDTO.FromQQ, "快晴"))
            {
                MsgSender.PushMsg(MsgDTO, "你无法进行该操作！(快晴)");
                return false;
            }

            var record = DriftItemRecord.GetRecord(MsgDTO.FromQQ);
            var ics = record.ItemCount?.Where(p => p.Count > 1).ToList();
            if (ics == null || ics.Count == 0)
            {
                MsgSender.PushMsg(MsgDTO, "你没有任何多余的物品！");
                return false;
            }

            var ictm = ics.Select(p => new
            {
                p.Name,
                Count = p.Count - 1,
                IsLimit = HonorHelper.Instance.IsLimit(p.Name),
                Price = HonorHelper.GetItemPrice(HonorHelper.Instance.FindItem(p.Name), MsgDTO.FromQQ)
            }).ToList();
            var msg = $"你即将贩卖{ictm.Sum(i => i.Count)}件物品，" +
                      $"其中有{ictm.Count(i => i.IsLimit)}件限定物品，" +
                      $"共价值{ictm.Sum(p => p.Price)}金币，是否继续？";
            if (!Waiter.Instance.WaitForConfirm(MsgDTO, msg, 7))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            foreach (var ic in ictm)
            {
                record.ItemConsume(ic.Name, ic.Count);
            }
            record.Update();

            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            osPerson.Golds += ictm.Sum(p => p.Price);
            osPerson.Update();

            MsgSender.PushMsg(MsgDTO, $"贩卖成功，你当前拥有{osPerson.Golds}金币！");
            return true;
        }

        private static void SellItem(MsgInformationEx MsgDTO, DriftBottleItemModel item)
        {
            var record = DriftItemRecord.GetRecord(MsgDTO.FromQQ);
            if (!record.CheckItem(item.Name))
            {
                MsgSender.PushMsg(MsgDTO, "你的背包里没有该物品！");
                return;
            }

            var price = HonorHelper.GetItemPrice(item, MsgDTO.FromQQ);
            var msg = $"贩卖此物品将获得 {price} 金币，是否确认贩卖？";
            if (!Waiter.Instance.WaitForConfirm(MsgDTO, msg, 7))
            {
                MsgSender.PushMsg(MsgDTO, "交易取消！");
                return;
            }

            var golds = TransHelper.SellItemToShop(MsgDTO.FromQQ, item.Name);
            MsgSender.PushMsg(MsgDTO, $"贩卖成功！你当前拥有金币 {golds}");
        }

        private static void SellHonor(MsgInformationEx MsgDTO, string honorName)
        {
            var query = MongoService<DriftItemRecord>.GetOnly(r => r.QQNum == MsgDTO.FromQQ);
            if (query == null || query.ItemCount.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "你尚未拥有任何物品！");
                return;
            }

            var items = HonorHelper.Instance.FindHonor(honorName).Items;
            var itemsOwned = query.ItemCount.Where(ic => items.Any(i => i.Name == ic.Name)).ToList();
            if (itemsOwned.Count < items.Count || itemsOwned.Any(io => io.Count <= 0))
            {
                MsgSender.PushMsg(MsgDTO, "你尚未集齐该成就下的所有物品！");
                return;
            }

            var price = HonorHelper.Instance.GetHonorPrice(honorName, MsgDTO.FromQQ);
            var msg = $"贩卖此成就将获得 {price} 金币，是否确认贩卖？";
            if (!Waiter.Instance.WaitForConfirm(MsgDTO, msg, 7))
            {
                MsgSender.PushMsg(MsgDTO, "交易取消！");
                return;
            }

            var golds = TransHelper.SellHonorToShop(query, MsgDTO.FromQQ, honorName);
            MsgSender.PushMsg(MsgDTO, $"贩卖成功！你当前拥有金币 {golds}");
        }

        [EnterCommand(ID = "ShoppingAI_ShopInfo",
            Command = "逛商店 .shopping",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取当前售卖的商品信息",
            Syntax = "",
            Tag = "商店功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true)]
        public bool ShopInfo(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            var golds = osPerson.Golds;

            var sellItems = TransHelper.GetDailySellItems();
            var record = DriftItemRecord.GetRecord(MsgDTO.FromQQ);
            var itemsStr = string.Join("\r", sellItems.Select(si =>
                $"{si.Name}({HonorHelper.Instance.FindHonorFullName(si.Name)})({record.GetCount(si.Name)})：{si.Price}金币"));

            var msg = $"今日售卖的商品：\r{itemsStr}\r你当前持有金币 {golds}";
            MsgSender.PushMsg(MsgDTO, msg);

            return true;
        }

        [EnterCommand(ID = "ShoppingAI_Buy",
            Command = "购买",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "购买指定（在商店中售卖的）商品",
            Syntax = "[商品名]",
            Tag = "商店功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true,
            DailyLimit = 5,
            TestingDailyLimit = 7)]
        public bool Buy(MsgInformationEx MsgDTO, object[] param)
        {
            if (OSPersonBuff.CheckBuff(MsgDTO.FromQQ, "快晴"))
            {
                MsgSender.PushMsg(MsgDTO, "你无法进行该操作！(快晴)");
                return false;
            }

            var name = param[0] as string;
            var sellItem = TransHelper.GetDailySellItems().FirstOrDefault(si => si.Name == name);
            if (sellItem == null)
            {
                MsgSender.PushMsg(MsgDTO, "此物品未在商店中售卖！");
                return false;
            }

            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (osPerson.Golds < sellItem.Price)
            {
                MsgSender.PushMsg(MsgDTO, "你持有的金币不足以购买此物品！");
                return false;
            }

            var price = OSPersonBuff.CheckBuff(MsgDTO.FromQQ, "极光") ? sellItem.Price * 80 / 100 : sellItem.Price;
            if (!Waiter.Instance.WaitForConfirm_Gold(MsgDTO, price, 7))
            {
                MsgSender.PushMsg(MsgDTO, "交易取消！");
                return false;
            }

            var record = DriftItemRecord.GetRecord(MsgDTO.FromQQ);
            var incomeMsg = record.ItemIncome(sellItem.Name);
            if (!string.IsNullOrEmpty(incomeMsg))
            {
                MsgSender.PushMsg(MsgDTO, incomeMsg, true);
            }

            OSPerson.GoldConsume(osPerson.QQNum, price);

            MsgSender.PushMsg(MsgDTO, $"购买成功！你当前剩余的金币为 {osPerson.Golds - sellItem.Price}");
            return true;
        }

        [EnterCommand(ID = "ShoppingAI_DealWith",
            Command = "交易",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "向另一个成员求购一个物品，并指定价格(系统将收取5%的手续费)",
            Syntax = "[@QQ号] [商品名] [价格]",
            Tag = "商店功能",
            SyntaxChecker = "At Word Long",
            IsPrivateAvailable = false,
            DailyLimit = 4,
            TestingDailyLimit = 5)]
        public bool DealWith(MsgInformationEx MsgDTO, object[] param)
        {
            try
            {
                if (OSPersonBuff.CheckBuff(MsgDTO.FromQQ, "快晴"))
                {
                    MsgSender.PushMsg(MsgDTO, "你无法进行该操作！(快晴)");
                    return false;
                }

                var aimQQ = (long) param[0];
                var itemName = param[1] as string;
                var price = (int)(long) param[2];

                if (aimQQ == MsgDTO.FromQQ)
                {
                    MsgSender.PushMsg(MsgDTO, "你无法和自己交易！");
                    return false;
                }

                if (price <= 0)
                {
                    MsgSender.PushMsg(MsgDTO, "价格异常！");
                    return false;
                }

                var aimRecord = DriftItemRecord.GetRecord(aimQQ);
                if (!aimRecord.CheckItem(itemName))
                {
                    MsgSender.PushMsg(MsgDTO, "对方没有该物品！");
                    return false;
                }

                var originPrice = HonorHelper.GetItemPrice(HonorHelper.Instance.FindItem(itemName), aimQQ);
                if (originPrice > price)
                {
                    MsgSender.PushMsg(MsgDTO, $"收购价格无法低于系统价格({originPrice})！");
                    return false;
                }

                var sourceOSPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
                var fee = OSPersonBuff.CheckBuff(MsgDTO.FromQQ, "苍天") ? 0 : price / 20;
                if (sourceOSPerson.Golds < price + fee)
                {
                    MsgSender.PushMsg(MsgDTO, "你没有足够的金币来支付！");
                    return false;
                }

                var count = aimRecord.GetCount(itemName);
                var msg = $"收到来自 {CodeApi.Code_At(MsgDTO.FromQQ)} 的交易请求：\r" +
                          $"希望得到的物品：{itemName}\r" +
                          $"价格：{price}({originPrice})\r" +
                          $"你当前持有：{count}个，是否确认交易？";
                if (!Waiter.Instance.WaitForConfirm(MsgDTO.FromGroup, aimQQ, msg,MsgDTO.BindAi, 10))
                {
                    MsgSender.PushMsg(MsgDTO, "交易取消！");
                    return false;
                }

                aimRecord.ItemConsume(itemName);
                aimRecord.Update();

                var sourceRecord = DriftItemRecord.GetRecord(MsgDTO.FromQQ);
                var content = sourceRecord.ItemIncome(itemName);
                if (!string.IsNullOrEmpty(content))
                {
                    MsgSender.PushMsg(MsgDTO, content, true);
                }

                var aimOSPerson = OSPerson.GetPerson(aimQQ);
                sourceOSPerson.Golds -= price + fee;
                aimOSPerson.Golds += price;

                sourceOSPerson.Update();
                aimOSPerson.Update();

                MsgSender.PushMsg(MsgDTO, "交易完毕！");
                return true;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return false;
            }
        }

        [EnterCommand(ID = "ShoppingAI_ViewItem",
            Command = "我的状态",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看自己的当前的状态（包括金币，物品数量，buff等）",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "商店功能",
            IsPrivateAvailable = true)]
        public bool ViewItem(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            var itemRecord = DriftItemRecord.GetRecord(MsgDTO.FromQQ);
            var advPlayer = AdvPlayer.GetPlayer(MsgDTO.FromQQ);
            var glamourRecord = GlamourRecord.Get(MsgDTO.FromGroup, MsgDTO.FromQQ);

            var msg = $"等级：{advPlayer.Level}\r" +
                      $"经验值：{advPlayer.Exp}\r" +
                      $"金币：{osPerson.Golds}\r" +
                      $"战绩：{advPlayer.WinTotal}/{advPlayer.GameTotal}\r" +
                      $"物品数量：{itemRecord.TotalItemCount()}\r" +
                      $"成就数量：{itemRecord.HonorList?.Count ?? 0}\r" +
                      $"魅力值：{glamourRecord.Glamour}";
            var buffs = OSPersonBuff.Get(MsgDTO.FromQQ);
            if (!buffs.IsNullOrEmpty())
            {
                msg += "\rBuff列表：\r" + string.Join("\r",
                           buffs.Select(b => $"{b.Name}：{b.Description}（{b.ExpiryTime.ToLocalTime()}）"));
            }

            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }

        [EnterCommand(ID = "ShoppingAI_Present",
            Command = "赠送",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "赠送一件物品给其他成员，需要支付5%的手续费",
            Syntax = "[@QQ号] [物品名]",
            Tag = "商店功能",
            SyntaxChecker = "At Word",
            IsPrivateAvailable = false,
            DailyLimit = 1,
            TestingDailyLimit = 1)]
        public bool Present(MsgInformationEx MsgDTO, object[] param)
        {
            var aimNum = (long) param[0];
            var name = param[1] as string;

            var sourceRecord = DriftItemRecord.GetRecord(MsgDTO.FromQQ);
            if (!sourceRecord.CheckItem(name))
            {
                MsgSender.PushMsg(MsgDTO, "你没有此物品", true);
                return false;
            }

            var itemModel = HonorHelper.Instance.FindItem(name);
            var price = HonorHelper.GetItemPrice(itemModel, MsgDTO.FromQQ) * 5 / 100;
            if (!Waiter.Instance.WaitForConfirm_Gold(MsgDTO, price, 7))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消!");
                return false;
            }

            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            osPerson.Golds -= price;
            osPerson.Update();

            sourceRecord.ItemConsume(name);
            sourceRecord.Update();
            var aimRecord = DriftItemRecord.GetRecord(aimNum);
            var msg = aimRecord.ItemIncome(name);

            var res = "赠送成功！";
            if (!string.IsNullOrEmpty(msg))
            {
                res += $"\r{msg}";
            }
            MsgSender.PushMsg(MsgDTO, res);

            return true;
        }
    }
}
