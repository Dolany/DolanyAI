using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.API;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.Model;
using Dolany.Database;
using Dolany.Database.Ai;
using Dolany.Game.OnlineStore;

namespace Dolany.Ai.Core.Ai.Game.Shopping
{
    [AI(
        Name = "商店",
        Description = "AI for Shopping.",
        Enable = true,
        PriorityLevel = 10)]
    public class ShoppingAI : AIBase
    {
        [EnterCommand(Command = "贩卖 出售",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "贩卖物品或者成就",
            Syntax = "[物品名或成就名]",
            Tag = "商店功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = false,
            DailyLimit = 6,
            TestingDailyLimit = 8)]
        public bool Sell(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (osPerson.CheckBuff("快晴"))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你无法进行该操作！(快晴)");
                return false;
            }

            var name = param[0] as string;

            var item = HonorHelper.Instance.FindItem(name);
            if (item != null)
            {
                SellItem(MsgDTO, item);
                return true;
            }

            if (!HonorHelper.Instance.FindHonorItems(name).IsNullOrEmpty())
            {
                SellHonor(MsgDTO, name);
                return true;
            }

            MsgSender.Instance.PushMsg(MsgDTO, "未查找到相关物品或成就！");
            return false;
        }

        private void SellItem(MsgInformationEx MsgDTO, DriftBottleItemModel item)
        {
            if (!ItemHelper.Instance.CheckItem(MsgDTO.FromQQ, item.Name))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你的背包里没有该物品！");
                return;
            }

            var price = HonorHelper.Instance.GetItemPrice(item, MsgDTO.FromQQ);
            var msg = $"贩卖此物品将获得 {price} 金币，是否确认贩卖？";
            if (!Waiter.Instance.WaitForConfirm(MsgDTO, msg, 7))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "交易取消！");
                return;
            }

            var golds = TransHelper.SellItemToShop(MsgDTO.FromQQ, item.Name);
            MsgSender.Instance.PushMsg(MsgDTO, $"贩卖成功！你当前拥有金币 {golds}");
        }

        private void SellHonor(MsgInformationEx MsgDTO, string honorName)
        {
            var query = MongoService<DriftItemRecord>.Get(r => r.QQNum == MsgDTO.FromQQ).FirstOrDefault();
            if (query == null || query.ItemCount.IsNullOrEmpty())
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你尚未拥有任何物品！");
                return;
            }

            var items = HonorHelper.Instance.FindHonorItems(honorName);
            var itemsOwned = query.ItemCount.Where(ic => items.Any(i => i.Name == ic.Name)).ToList();
            if (itemsOwned.Count < items.Length || itemsOwned.Any(io => io.Count <= 0))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你尚未集齐该成就下的所有物品！");
                return;
            }

            var price = HonorHelper.Instance.GetHonorPrice(honorName, MsgDTO.FromQQ);
            var msg = $"贩卖此成就将获得 {price} 金币，是否确认贩卖？";
            if (!Waiter.Instance.WaitForConfirm(MsgDTO, msg, 7))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "交易取消！");
                return;
            }

            var golds = TransHelper.SellHonorToShop(query, MsgDTO.FromQQ, honorName);
            MsgSender.Instance.PushMsg(MsgDTO, $"贩卖成功！你当前拥有金币 {golds}");
        }

        [EnterCommand(Command = "逛商店 .shopping",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取当前售卖的商品信息",
            Syntax = "",
            Tag = "商店功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public bool ShopInfo(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            var golds = osPerson.Golds;

            var sellItems = TransHelper.GetDailySellItems();
            var record = DriftItemRecord.GetRecord(MsgDTO.FromQQ);
            var itemsStr = string.Join("\r", sellItems.Select(si =>
                $"{si.Name}({HonorHelper.Instance.FindHonor(si.Name)})({ItemHelper.Instance.ItemCount(record, si.Name)})：{si.Price}金币"));

            var msg = $"今日售卖的商品：\r{itemsStr}\r你当前持有金币 {golds}";
            MsgSender.Instance.PushMsg(MsgDTO, msg);

            return true;
        }

        [EnterCommand(Command = "购买",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "购买指定（在商店中售卖的）商品",
            Syntax = "[商品名]",
            Tag = "商店功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = false,
            DailyLimit = 5,
            TestingDailyLimit = 7)]
        public bool Buy(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (osPerson.CheckBuff("快晴"))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你无法进行该操作！(快晴)");
                return false;
            }

            var name = param[0] as string;
            var sellItem = TransHelper.GetDailySellItems().FirstOrDefault(si => si.Name == name);
            if (sellItem == null)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "此物品未在商店中售卖！");
                return false;
            }

            if (osPerson.Golds < sellItem.Price)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你持有的金币不足以购买此物品！");
                return false;
            }

            var price = osPerson.CheckBuff("极光") ? sellItem.Price * 80 / 100 : sellItem.Price;
            var msg = $"购买此物品将消耗 {price} 金币，是否确认购买？";
            if (!Waiter.Instance.WaitForConfirm(MsgDTO, msg, 7))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "交易取消！");
                return false;
            }

            var (incomeMsg, _) = ItemHelper.Instance.ItemIncome(MsgDTO.FromQQ, sellItem.Name);
            if (!string.IsNullOrEmpty(incomeMsg))
            {
                MsgSender.Instance.PushMsg(MsgDTO, incomeMsg, true);
            }

            OSPerson.GoldConsume(osPerson.QQNum, price);

            MsgSender.Instance.PushMsg(MsgDTO, $"购买成功！你当前剩余的金币为 {osPerson.Golds - sellItem.Price}");
            return true;
        }

        [EnterCommand(Command = "交易",
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
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (osPerson.CheckBuff("快晴"))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你无法进行该操作！(快晴)");
                return false;
            }

            var aimQQ = (long) param[0];
            var itemName = param[1] as string;
            var price = (int)(long) param[2];

            if (aimQQ == MsgDTO.FromQQ)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你无法和自己交易！");
                return false;
            }

            if (price <= 0)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "价格异常！");
                return false;
            }

            if (!ItemHelper.Instance.CheckItem(aimQQ, itemName))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "对方没有该物品！");
                return false;
            }

            var originPrice = HonorHelper.Instance.GetItemPrice(HonorHelper.Instance.FindItem(itemName), aimQQ);
            if (originPrice > price)
            {
                MsgSender.Instance.PushMsg(MsgDTO, $"收购价格无法低于系统价格({originPrice})！(打击黑心商人！)");
                return false;
            }

            var sourceOSPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            var fee = sourceOSPerson.CheckBuff("苍天") ? 0 : price / 20;
            if (sourceOSPerson.Golds < price + fee)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你没有足够的金币来支付！");
                return false;
            }

            var count = ItemHelper.Instance.ItemCount(aimQQ, itemName);
            var msg = $"收到来自 {CodeApi.Code_At(MsgDTO.FromQQ)} 的交易请求：\r" +
                      $"希望得到的物品：{itemName}\r" +
                      $"价格：{price}({originPrice})\r" +
                      $"你当前持有：{count}个，是否确认交易？";
            if (!Waiter.Instance.WaitForConfirm(MsgDTO.FromGroup, aimQQ, msg, 10))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "交易取消！");
                return false;
            }

            ItemHelper.Instance.ItemConsume(aimQQ, itemName);
            var (content, _) = ItemHelper.Instance.ItemIncome(MsgDTO.FromQQ, itemName);
            if (!string.IsNullOrEmpty(content))
            {
                MsgSender.Instance.PushMsg(MsgDTO, content, true);
            }

            var aimOSPerson = OSPerson.GetPerson(aimQQ);
            sourceOSPerson.Golds -= price + fee;
            aimOSPerson.Golds += price;

            sourceOSPerson.Update();
            aimOSPerson.Update();

            MsgSender.Instance.PushMsg(MsgDTO, "交易完毕！");
            return true;
        }

        [EnterCommand(Command = "我的状态",
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

            var msg = $"金币：{osPerson.Golds}\r" + $"物品数量：{itemRecord.ItemCount?.Count ?? 0}\r" +
                      $"成就数量：{itemRecord.HonorList?.Count ?? 0}";
            var buffs = osPerson.EffectiveBuffs;
            if (!buffs.IsNullOrEmpty())
            {
                msg += "\rBuff列表：\r" + string.Join("\r", buffs.Select(b => $"{b.Name}：{b.Description}（{b.ExpiryTime.ToLocalTime()}）"));
            }

            MsgSender.Instance.PushMsg(MsgDTO, msg, true);
            return true;
        }
    }
}
