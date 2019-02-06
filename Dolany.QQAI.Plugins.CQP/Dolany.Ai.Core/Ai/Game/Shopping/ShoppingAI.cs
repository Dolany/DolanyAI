using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.API;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Model;
using Dolany.Database;
using Dolany.Database.Ai;
using Dolany.Database.Sqlite;
using Dolany.Game.OnlineStore;

namespace Dolany.Ai.Core.Ai.Game.Shopping
{
    [AI(
        Name = nameof(ShoppingAI),
        Description = "AI for Shopping.",
        Enable = true,
        PriorityLevel = 10)]
    public class ShoppingAI : AIBase
    {
        [EnterCommand(Command = "贩卖",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "贩卖物品或者成就",
            Syntax = "[物品名或成就名]",
            Tag = "商店功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = false)]
        public void Sell(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;

            var item = HonorHelper.Instance.FindItem(name);
            if (item != null)
            {
                SellItem(MsgDTO, item);
                return;
            }

            if (!HonorHelper.Instance.FindHonorItems(name).IsNullOrEmpty())
            {
                SellHonor(MsgDTO, name);
                return;
            }

            MsgSender.Instance.PushMsg(MsgDTO, "未查找到相关物品或成就！");
        }

        private void SellItem(MsgInformationEx MsgDTO, DriftBottleItemModel item)
        {
            if (!ItemHelper.Instance.CheckItem(MsgDTO.FromQQ, item.Name))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你的背包里没有该物品！");
                return;
            }

            var price = HonorHelper.Instance.GetItemPrice(item);
            var msg = $"贩卖此物品将获得 {price} 金币，是否确认贩卖？";
            if (!Waiter.Instance.WaitForConfirm(MsgDTO, msg, 7))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "交易取消！");
                return;
            }

            OSPerson.GoldIncome(MsgDTO.FromQQ, price);

            ItemHelper.Instance.ItemConsume(MsgDTO.FromQQ, item.Name);

            MsgSender.Instance.PushMsg(MsgDTO, $"贩卖成功！你当前拥有金币 {OSPerson.GetPerson(MsgDTO.FromQQ).Golds}");
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

            var price = GetHonorPrice(honorName);
            var msg = $"贩卖此成就将获得 {price} 金币，是否确认贩卖？";
            if (!Waiter.Instance.WaitForConfirm(MsgDTO, msg, 7))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "交易取消！");
                return;
            }

            OSPerson.GoldIncome(MsgDTO.FromQQ, price);

            foreach (var record in itemsOwned)
            {
                record.Count--;
                if (record.Count <= 0)
                {
                    query.ItemCount.Remove(record);
                }
            }

            query.HonorList.Remove(honorName);

            MongoService<DriftItemRecord>.Update(query);

            MsgSender.Instance.PushMsg(MsgDTO, $"贩卖成功！你当前拥有金币 {OSPerson.GetPerson(MsgDTO.FromQQ).Golds}");
        }

        [EnterCommand(Command = "逛商店 .shopping",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取当前售卖的商品信息",
            Syntax = "",
            Tag = "商店功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public void ShopInfo(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            var golds = osPerson.Golds;

            var sellItems = GetDailySellItems();
            var itemsStr = string.Join("\r", sellItems.Select(si => $"商品名：{si.Name}({HonorHelper.Instance.FindHonor(si.Name)}), 售价：{si.Price}"));
            var msg = $"今日售卖的商品：\r{itemsStr}\r你当前持有金币 {golds}";
            MsgSender.Instance.PushMsg(MsgDTO, msg);
        }

        private int GetHonorPrice(string honorName)
        {
            var items = HonorHelper.Instance.FindHonorItems(honorName);
            return items.Sum(HonorHelper.Instance.GetItemPrice) * 3 / 2;
        }

        private DailySellItemModel[] GetDailySellItems()
        {
            const string key = "DailySellItems";
            var cache = SCacheService.Get<DailySellItemModel[]>(key);
            if (cache != null)
            {
                return cache;
            }

            var newitems = CreateDailySellItems();
            SCacheService.Cache(key, newitems);

            return newitems;
        }

        private DailySellItemModel[] CreateDailySellItems()
        {
            var randSort = CommonUtil.RandSort(HonorHelper.Instance.Items.ToArray()).Take(5);
            return randSort.Select(rs => new DailySellItemModel {Name = rs.Name, Price = HonorHelper.Instance.GetItemPrice(rs) * 2}).ToArray();
        }

        [EnterCommand(Command = "购买",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "购买指定（在商店中售卖的）商品",
            Syntax = "[商品名]",
            Tag = "商店功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = false)]
        public void Buy(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var sellItem = GetDailySellItems().FirstOrDefault(si => si.Name == name);
            if (sellItem == null)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "此物品未在商店中售卖！");
                return;
            }

            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);

            if (osPerson.Golds < sellItem.Price)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你持有的金币不足以购买此物品！");
                return;
            }

            var msg = $"购买此物品将消耗 {sellItem.Price} 金币，是否确认购买？";
            if (!Waiter.Instance.WaitForConfirm(MsgDTO, msg, 7))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "交易取消！");
                return;
            }

            var imsg = ItemHelper.Instance.ItemIncome(MsgDTO.FromQQ, sellItem.Name);
            if (!string.IsNullOrEmpty(imsg.msg))
            {
                MsgSender.Instance.PushMsg(MsgDTO, imsg.msg, true);
            }

            OSPerson.GoldConsume(osPerson.QQNum, sellItem.Price);

            MsgSender.Instance.PushMsg(MsgDTO, $"购买成功！你当前剩余的金币为 {osPerson.Golds - sellItem.Price}");
        }

        [EnterCommand(Command = "交易",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "向另一个成员求购一个物品，并指定价格",
            Syntax = "[@QQ号] [商品名] [价格]",
            Tag = "商店功能",
            SyntaxChecker = "At Word Long",
            IsPrivateAvailable = false)]
        public void DealWith(MsgInformationEx MsgDTO, object[] param)
        {
            var aimQQ = (long) param[0];
            var itemName = param[1] as string;
            var price = (int)(long) param[2];

            if (price <= 0)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "价格异常！");
                return;
            }

            var sourceOSPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (sourceOSPerson.Golds < price)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你没有足够的金币来支付！");
                return;
            }

            if (!ItemHelper.Instance.CheckItem(aimQQ, itemName))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "对方没有该物品！");
                return;
            }

            var count = ItemHelper.Instance.ItemCount(aimQQ, itemName);
            var msg = $"{CodeApi.Code_At(aimQQ)} 收到来自 {CodeApi.Code_At(MsgDTO.FromQQ)} 的交易请求：\r" +
                      $"希望得到的物品：{itemName}\r" +
                      $"价格：{price}\r" +
                      $"你当前持有：{count}个，是否确认交易？";
            if (!Waiter.Instance.WaitForConfirm(MsgDTO.FromGroup, aimQQ, msg, 10))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "交易取消！");
                return;
            }

            ItemHelper.Instance.ItemConsume(aimQQ, itemName);
            var (content, record) = ItemHelper.Instance.ItemIncome(MsgDTO.FromQQ, itemName);
            if (!string.IsNullOrEmpty(content))
            {
                MsgSender.Instance.PushMsg(MsgDTO, content, true);
            }

            var aimOSPerson = OSPerson.GetPerson(aimQQ);
            sourceOSPerson.Golds -= price;
            aimOSPerson.Golds += price;

            MongoService<OSPerson>.Update(sourceOSPerson);
            MongoService<OSPerson>.Update(aimOSPerson);

            MsgSender.Instance.PushMsg(MsgDTO, "交易完毕！");
        }
    }

    public class DailySellItemModel
    {
        public string Name { get; set; }

        public int Price { get; set; }
    }
}
