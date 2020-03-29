using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Database.Ai;
using Dolany.WorldLine.Standard.Ai.Game.Gift;
using Dolany.WorldLine.Standard.Ai.Game.Pet;
using Dolany.WorldLine.Standard.Ai.Game.Pet.Cooking;
using Dolany.WorldLine.Standard.Ai.Vip;
using Dolany.WorldLine.Standard.OnlineStore;

namespace Dolany.WorldLine.Standard.Ai.Game.Shopping
{
    public class ShoppingAI : AIBase
    {
        public override string AIName { get; set; } = "商店";

        public override string Description { get; set; } = "AI for Shopping.";

        public override CmdTagEnum DefaultTag { get; } = CmdTagEnum.商店功能;

        private const int RebornHonorLimit = 7;

        public DailyVipShopSvc DailyVipShopSvc { get; set; }
        public CookingDietSvc CookingDietSvc { get; set; }
        public GiftSvc GiftSvc { get; set; }
        public PetLevelSvc PetLevelSvc { get; set; }
        public HonorSvc HonorSvc { get; set; }

        [EnterCommand(ID = "ShoppingAI_Sell",
            Command = "贩卖 出售 贩卖物品 出售物品",
            Description = "贩卖物品或者成就",
            SyntaxHint = "[物品名或成就名]",
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

            var item = HonorSvc.FindItem(name);
            if (item != null)
            {
                return SellItem(MsgDTO, item);
            }

            if (HonorSvc.FindHonor(name) != null)
            {
                return SellHonor(MsgDTO, name);
            }

            MsgSender.PushMsg(MsgDTO, "未查找到相关物品或成就！");
            return false;
        }

        [EnterCommand(ID = "ShoppingAI_SellHonor",
            Command = "贩卖成就 出售成就",
            Description = "贩卖指定成就",
            SyntaxHint = "[成就名]",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true,
            DailyLimit = 6,
            TestingDailyLimit = 8)]
        public bool SellHonor(MsgInformationEx MsgDTO, object[] param)
        {
            if (OSPersonBuff.CheckBuff(MsgDTO.FromQQ, "快晴"))
            {
                MsgSender.PushMsg(MsgDTO, "你无法进行该操作！(快晴)");
                return false;
            }

            var name = param[0] as string;

            if (HonorSvc.FindHonor(name) != null)
            {
                return SellHonor(MsgDTO, name);
            }

            MsgSender.PushMsg(MsgDTO, "未查找到相关成就！");
            return false;
        }

        [EnterCommand(ID = "ShoppingAI_SellMulti",
            Command = "贩卖 出售",
            Description = "批量贩卖物品",
            SyntaxHint = "[物品名] [物品数量]",
            SyntaxChecker = "Word Long",
            IsPrivateAvailable = true,
            DailyLimit = 3,
            TestingDailyLimit = 5)]
        public bool SellMulti(MsgInformationEx MsgDTO, object[] param)
        {
            if (OSPersonBuff.CheckBuff(MsgDTO.FromQQ, "快晴"))
            {
                MsgSender.PushMsg(MsgDTO, "你无法进行该操作！(快晴)");
                return false;
            }

            var name = param[0] as string;
            var count = (int) (long) param[1];
            if (count <= 0)
            {
                MsgSender.PushMsg(MsgDTO, "错误的物品数量！");
                return false;
            }

            var item = HonorSvc.FindItem(name);
            if (item != null)
            {
                SellItem(MsgDTO, item, count);
                return true;
            }

            MsgSender.PushMsg(MsgDTO, "未查找到相关物品！");
            return false;
        }

        [EnterCommand(ID = "ShoppingAI_SellRedundant",
            Command = "贩卖多余物品 出售多余物品",
            Description = "一键贩卖自己多余的物品",
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

            var record = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var ics = record.HonorCollections.Values.SelectMany(hc => hc.Items.Where(p => p.Value > 1)).ToList();
            if (ics.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "你没有任何多余的物品！");
                return false;
            }

            var ictm = ics.Select(p => new
            {
                p.Key,
                Count = p.Value - 1,
                IsLimit = HonorSvc.IsLimitItem(p.Key),
                Price = HonorSvc.GetItemPrice(HonorSvc.FindItem(p.Key), MsgDTO.FromQQ)
            }).ToList();
            var msg = $"你即将贩卖{ictm.Sum(i => i.Count)}件物品，" +
                      $"其中有{ictm.Count(i => i.IsLimit)}件限定物品，" +
                      $"共价值{ictm.Sum(p => p.Price * p.Count).CurencyFormat()}，是否继续？";
            if (!WaiterSvc.WaitForConfirm(MsgDTO, msg))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            foreach (var ic in ictm)
            {
                record.ItemConsume(ic.Key, ic.Count);
            }
            record.Update();

            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            osPerson.Golds += ictm.Sum(p => p.Price * p.Count);
            osPerson.Update();

            MsgSender.PushMsg(MsgDTO, $"贩卖成功，你当前拥有{osPerson.Golds.CurencyFormat()}！");
            return true;
        }

        private bool SellItem(MsgInformationEx MsgDTO, DriftBottleItemModel item, int count = 1)
        {
            var record = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            if (!record.CheckItem(item.Name, count))
            {
                MsgSender.PushMsg(MsgDTO, "你的背包里没有足够多的该物品！");
                return false;
            }

            var price = HonorSvc.GetItemPrice(item, MsgDTO.FromQQ);
            var msg = $"贩卖 {item.Name}*{count} 将获得 {(price * count).CurencyFormat()}，是否确认贩卖？";
            if (!WaiterSvc.WaitForConfirm(MsgDTO, msg))
            {
                MsgSender.PushMsg(MsgDTO, "交易取消！");
                return false;
            }

            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            TransHelper.SellItemToShop(item.Name, osPerson, count);
            osPerson.Update();

            MsgSender.PushMsg(MsgDTO, $"贩卖成功！你当前拥有金币 {osPerson.Golds.CurencyFormat()}");
            return true;
        }

        private bool SellHonor(MsgInformationEx MsgDTO, string honorName)
        {
            var colleRec = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            if (colleRec.HonorCollections.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "你尚未拥有任何物品！");
                return false;
            }

            var items = HonorSvc.FindHonor(honorName).Items;
            var honorCollection = colleRec.HonorCollections;
            if (!honorCollection.ContainsKey(honorName) || honorCollection[honorName].Items.Count < items.Count)
            {
                MsgSender.PushMsg(MsgDTO, "你尚未集齐该成就下的所有物品！");
                return false;
            }

            var price = HonorSvc.GetHonorPrice(honorName, MsgDTO.FromQQ);
            var msg = $"贩卖此成就将获得 {price.CurencyFormat()}，是否确认贩卖？";
            if (!WaiterSvc.WaitForConfirm(MsgDTO, msg))
            {
                MsgSender.PushMsg(MsgDTO, "交易取消！");
                return false;
            }

            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            TransHelper.SellHonorToShop(colleRec, honorName, osPerson);

            colleRec.Update();
            osPerson.Update();

            MsgSender.PushMsg(MsgDTO, $"贩卖成功！你当前拥有金币 {osPerson.Golds.CurencyFormat()}");
            return true;
        }

        [EnterCommand(ID = "ShoppingAI_ShopInfo",
            Command = "逛商店 .shopping",
            Description = "获取当前售卖的商品信息",
            IsPrivateAvailable = true)]
        public bool ShopInfo(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            var golds = osPerson.Golds;

            var sellItems = TransHelper.GetDailySellItems();
            var record = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var itemsStr = string.Join("\r\n", sellItems.Select(si =>
                $"{si.Name}({HonorSvc.FindHonorFullName(si.Name)})({record.GetCount(si.Name)})({si.Attr})：{si.Price.CurencyFormat()}"));

            var msg = $"今日售卖的商品：\r\n{itemsStr}\r\n你当前持有 {golds.CurencyFormat()}";
            MsgSender.PushMsg(MsgDTO, msg);

            return true;
        }

        [EnterCommand(ID = "ShoppingAI_ShopInfo_Rare",
            Command = "稀有商店",
            Description = "逛稀有商店(每日随机开放三个小时)",
            IsPrivateAvailable = true)]
        public bool ShopInfo_Rare(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            var golds = osPerson.Golds;

            var todayRec = DailySellItemRareRecord.GetToday();
            var tomorrowRec = DailySellItemRareRecord.GetTomorrow();
            if (todayRec.IsOver)
            {
                MsgSender.PushMsg(MsgDTO,
                    $"稀有商店休息中~\r\n下次开放时间：明天 {tomorrowRec.Hour}:00:00 ~ {tomorrowRec.Hour + 3}:00:00");
                return false;
            }

            if (todayRec.IsBefore)
            {
                MsgSender.PushMsg(MsgDTO,
                    $"稀有商店休息中~\r\n下次开放时间：今天 {todayRec.Hour}:00:00 ~ {todayRec.Hour + 3}:00:00");
                return false;
            }

            var sellItems = todayRec.Items;
            var record = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var itemsStr = string.Join("\r\n", sellItems.Select(si =>
                $"{si.Name}({HonorSvc.FindHonorFullName(si.Name)})({record.GetCount(si.Name)})({si.Attr})：{si.Price.CurencyFormat()}"));

            var msg = $"当前售卖的商品：\r\n{itemsStr}\r\n你当前持有 {golds.CurencyFormat()}";
            MsgSender.PushMsg(MsgDTO, msg);

            return true;
        }

        [EnterCommand(ID = "ShoppingAI_Buy",
            Command = "购买 购买物品",
            Description = "购买指定（在商店中售卖的）商品",
            SyntaxHint = "[商品名]",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true,
            DailyLimit = 5,
            TestingDailyLimit = 7)]
        public bool Buy(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var vipSvc = DailyVipShopSvc[name];
            if (vipSvc != null)
            {
                DailyVipShopSvc.Serve(MsgDTO, name);
                return false;
            }

            if (OSPersonBuff.CheckBuff(MsgDTO.FromQQ, "快晴"))
            {
                MsgSender.PushMsg(MsgDTO, "你无法进行该操作！(快晴)");
                return false;
            }

            var sellingItems = TransHelper.GetDailySellItems();
            var todayRec = DailySellItemRareRecord.GetToday();
            if (DateTime.Now.Hour >= todayRec.Hour && DateTime.Now.Hour <= todayRec.Hour + 2)
            {
                sellingItems = sellingItems.Concat(todayRec.Items);
            }

            var sellItem = sellingItems.FirstOrDefault(si => si.Name == name);
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
            if (!WaiterSvc.WaitForConfirm_Gold(MsgDTO, price))
            {
                MsgSender.PushMsg(MsgDTO, "交易取消！");
                return false;
            }

            var record = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var incomeMsg = record.ItemIncome(sellItem.Name);

            OSPerson.GoldConsume(osPerson.QQNum, price);

            MsgSender.PushMsg(MsgDTO, $"{incomeMsg}\r\n购买成功！你当前剩余的金币为 {(osPerson.Golds - sellItem.Price).CurencyFormat()}");
            return true;
        }

        [EnterCommand(ID = "ShoppingAI_DealWith",
            Command = "交易",
            Description = "向另一个成员求购一个物品/菜肴，并指定价格(系统将收取5%的手续费)",
            SyntaxHint = "[@QQ号] [商品名/菜肴名] [价格]",
            SyntaxChecker = "At Word Long",
            DailyLimit = 4,
            TestingDailyLimit = 5)]
        public bool DealWith(MsgInformationEx MsgDTO, object[] param)
        {
            if (OSPersonBuff.CheckBuff(MsgDTO.FromQQ, "快晴"))
            {
                MsgSender.PushMsg(MsgDTO, "你无法进行该操作！(快晴)");
                return false;
            }

            var aimQQ = (long) param[0];
            var name = param[1] as string;
            var price = (int) (long) param[2];

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

            if (HonorSvc.FindItem(name) != null)
            {
                return DealItem(MsgDTO, name, aimQQ, price);
            }

            if (CookingDietSvc[name] != null)
            {
                return DealDiet(MsgDTO, name, aimQQ, price);
            }

            MsgSender.PushMsg(MsgDTO, "未查找到相关物品信息！");
            return false;
        }

        private bool DealItem(MsgInformationEx MsgDTO, string name, long aimQQ, int price)
        {
            var aimRecord = ItemCollectionRecord.Get(aimQQ);
            if (!aimRecord.CheckItem(name))
            {
                MsgSender.PushMsg(MsgDTO, "对方没有该物品！");
                return false;
            }

            var originPrice = HonorSvc.GetItemPrice(HonorSvc.FindItem(name), aimQQ);
            if (originPrice > price)
            {
                MsgSender.PushMsg(MsgDTO, $"收购价格无法低于系统价格({originPrice.CurencyFormat()})！");
                return false;
            }

            var sourceOSPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            var fee = OSPersonBuff.CheckBuff(MsgDTO.FromQQ, "苍天") ? 0 : price / 20;
            if (sourceOSPerson.Golds < price + fee)
            {
                MsgSender.PushMsg(MsgDTO, "你没有足够的金币来支付！");
                return false;
            }

            var count = aimRecord.GetCount(name);
            var msg = $"收到来自 {CodeApi.Code_At(MsgDTO.FromQQ)} 的交易请求：\r\n" +
                      $"希望得到的物品：{name}\r\n" +
                      $"价格：{price.CurencyFormat()}({originPrice.CurencyFormat()})\r\n" +
                      $"你当前持有：{count}个，是否确认交易？";
            if (!WaiterSvc.WaitForConfirm(MsgDTO.FromGroup, aimQQ, msg, MsgDTO.BindAi, 10))
            {
                MsgSender.PushMsg(MsgDTO, "交易取消！");
                return false;
            }

            aimRecord.ItemConsume(name);
            aimRecord.Update();

            var sourceRecord = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var content = sourceRecord.ItemIncome(name);
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

        private bool DealDiet(MsgInformationEx MsgDTO, string name, long aimQQ, int price)
        {
            var aimDietRec = CookingRecord.Get(aimQQ);
            if (!aimDietRec.CheckDiet(name))
            {
                MsgSender.PushMsg(MsgDTO, "对方没有该菜肴！");
                return false;
            }

            var dietModel = CookingDietSvc[name];
            if (dietModel.EstimatedPrice < price)
            {
                MsgSender.PushMsg(MsgDTO, $"交易价格不能低于该菜肴的成本价格({dietModel.EstimatedPrice.CurencyFormat()})！");
                return false;
            }

            var sourceOSPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            var fee = OSPersonBuff.CheckBuff(MsgDTO.FromQQ, "苍天") ? 0 : price / 20;
            if (sourceOSPerson.Golds < price + fee)
            {
                MsgSender.PushMsg(MsgDTO, "你没有足够的金币来支付！");
                return false;
            }

            var count = aimDietRec.CookedDietDic[name];
            var msg = $"收到来自 {CodeApi.Code_At(MsgDTO.FromQQ)} 的交易请求：\r\n" +
                      $"希望得到的菜肴：{name}\r\n" +
                      $"价格：{price.CurencyFormat()}({dietModel.EstimatedPrice.CurencyFormat()})\r\n" +
                      $"你当前持有：{count}个，是否确认交易？";
            if (!WaiterSvc.WaitForConfirm(MsgDTO.FromGroup, aimQQ, msg, MsgDTO.BindAi, 10))
            {
                MsgSender.PushMsg(MsgDTO, "交易取消！");
                return false;
            }

            var sourceDietRec = CookingRecord.Get(MsgDTO.FromQQ);
            sourceDietRec.AddDiet(name);
            sourceDietRec.Update();

            aimDietRec.DietConsume(name);
            aimDietRec.Update();

            sourceOSPerson.Golds -= price + fee;
            sourceOSPerson.Update();

            var aimOSPerson = OSPerson.GetPerson(aimQQ);
            aimOSPerson.Golds += price;
            aimOSPerson.Update();

            MsgSender.PushMsg(MsgDTO, "交易完毕！");
            return true;
        }

        [EnterCommand(ID = "ShoppingAI_ViewItem",
            Command = "我的状态",
            Description = "查看自己的当前的状态（包括金币，物品数量，buff等）",
            IsPrivateAvailable = true)]
        public bool ViewItem(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            var itemRecord = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var glamourRecord = GlamourRecord.Get(MsgDTO.FromGroup, MsgDTO.FromQQ);

            var normalHonors = itemRecord.HonorCollections.Where(h => h.Value.Type == HonorType.Normal).ToList();
            var items = normalHonors.Select(p => p.Value).SelectMany(h => h.Items.Keys).ToList();

            var allNormalItems = HonorSvc.HonorList.Where(h => !h.IsLimit).SelectMany(h => h.Items).Select(p => p.Name).ToArray();

            var msgList = new List<string>();
            if (!osPerson.HonorNames.IsNullOrEmpty())
            {
                msgList.Add(string.Join("", osPerson.HonorNames.Select(h => $"【{h}】")));
            }
            msgList.Add($"等级：{osPerson.EmojiLevel}");
            msgList.Add($"经验值：{items.Count}/{allNormalItems.Length}{(items.Count == allNormalItems.Length ? "(可转生)" : string.Empty)}");
            
            msgList.Add($"金币：{osPerson.Golds.CurencyFormat()}");
            if (osPerson.Diamonds > 0)
            {
                msgList.Add($"钻石：{osPerson.Diamonds.CurencyFormat("Diamond")}");
            }
            msgList.Add($"物品数量：{itemRecord.TotalItemCount()}");
            msgList.Add($"成就数量：{itemRecord.HonorList?.Count ?? 0}");
            if (glamourRecord.Glamour > 0)
            {
                msgList.Add($"魅力值：{glamourRecord.Glamour}");
            }

            var buffs = OSPersonBuff.Get(MsgDTO.FromQQ);
            if (!buffs.IsNullOrEmpty())
            {
                msgList.Add("Buff列表：");
                msgList.AddRange(buffs.Select(b => $"{b.Name}：{b.Description}（{b.ExpiryTime}）"));
            }

            MsgSender.PushMsg(MsgDTO, string.Join("\r\n", msgList), true);
            return true;
        }

        [EnterCommand(ID = "ShoppingAI_Present",
            Command = "赠送",
            Description = "赠送一件物品给其他成员，需要支付5%的手续费",
            SyntaxHint = "[@QQ号] [物品名]",
            SyntaxChecker = "At Word",
            DailyLimit = 1,
            TestingDailyLimit = 1)]
        public bool Present(MsgInformationEx MsgDTO, object[] param)
        {
            var aimNum = (long) param[0];
            var name = param[1] as string;

            var sourceRecord = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            if (!sourceRecord.CheckItem(name))
            {
                MsgSender.PushMsg(MsgDTO, "你没有此物品", true);
                return false;
            }

            var itemModel = HonorSvc.FindItem(name);
            var price = HonorSvc.GetItemPrice(itemModel, MsgDTO.FromQQ) * 5 / 100;
            if (!WaiterSvc.WaitForConfirm_Gold(MsgDTO, price))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消!");
                return false;
            }

            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            osPerson.Golds -= price;
            osPerson.Update();

            sourceRecord.ItemConsume(name);
            sourceRecord.Update();

            var aimRecord = ItemCollectionRecord.Get(aimNum);
            var msg = aimRecord.ItemIncome(name);

            var res = "赠送成功！";
            if (!string.IsNullOrEmpty(msg))
            {
                res += $"\r\n{msg}";
            }
            MsgSender.PushMsg(MsgDTO, res);

            return true;
        }

        [EnterCommand(ID = "ShoppingAI_AssertCalculate",
            Command = "资产评估",
            Description = "评估自己的资产情况",
            IsPrivateAvailable = true)]
        public bool AssertCalculate(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (osPerson.Golds < 50)
            {
                MsgSender.PushMsg(MsgDTO, $"你的金币余额不足({osPerson.Golds.CurencyFormat()}/{50.CurencyFormat()})!");
                return false;
            }

            if (!WaiterSvc.WaitForConfirm_Gold(MsgDTO, 50))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            osPerson.Golds -= 50;
            osPerson.Update();

            var resultDic = new Dictionary<string, int> {{"金币资产", osPerson.Golds}};

            var itemRecord = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            if (!itemRecord.HonorCollections.IsNullOrEmpty())
            {
                var itemAssert = itemRecord.AssertToGold();

                resultDic.Add("物品资产", itemAssert);
            }

            if (!osPerson.GiftDic.IsNullOrEmpty())
            {
                var giftsMaterialDic = osPerson.GiftDic.SelectMany(p => GiftSvc[p.Key].MaterialDic);
                var giftAssert = giftsMaterialDic.Sum(g => HonorSvc.FindItem(g.Key).Price * g.Value);
                resultDic.Add("礼物资产", giftAssert);
            }

            var pet = PetRecord.Get(MsgDTO.FromQQ);
            if (pet.Level > 0 || pet.Exp > 0)
            {
                var petAssert = PetLevelSvc.ExpToGolds(pet.Level, pet.Exp);
                resultDic.Add("宠物资产", petAssert);
            }

            var dietRec = CookingRecord.Get(MsgDTO.FromQQ);
            if (!dietRec.LearndDietMenu.IsNullOrEmpty() || !dietRec.CookedDietDic.IsNullOrEmpty() || !dietRec.FlavoringDic.IsNullOrEmpty())
            {
                var dietAssert = dietRec.LearndDietMenu.Sum(menu =>
                    HonorSvc.FindHonor(CookingDietSvc[menu].ExchangeHonor).Items.Sum(item => item.Price));
                dietAssert += dietRec.CookedDietDic.Sum(diet => CookingDietSvc[diet.Key].EstimatedPrice * diet.Value);
                dietAssert += dietRec.FlavoringDic.Sum(p => p.Value) * 20;
                resultDic.Add("烹饪资产", dietAssert);
            }

            var msg = "请查阅你的资产评估报告：\r\n" +
                      $"{string.Join("\r\n", resultDic.Select(rd => $"{rd.Key}:{rd.Value.CurencyFormat()}"))}" +
                      $"\r\n总资产:{resultDic.Sum(p => p.Value).CurencyFormat()}";
            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }

        [EnterCommand(ID = "ShoppingAI_Reborn",
            Command = "灵魂转生",
            Description = "献祭所有物品，等级+1，获取一个自定义荣誉称号",
            IsPrivateAvailable = true)]
        public bool Reborn(MsgInformationEx MsgDTO, object[] param)
        {
            var itemColl = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            if (itemColl.HonorCollections.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "你尚未集齐所有非限定物品！", true);
                return false;
            }

            var normalHonors = itemColl.HonorCollections.Where(h => h.Value.Type == HonorType.Normal).ToList();
            var items = normalHonors.Select(p => p.Value).SelectMany(h => h.Items.Keys).ToList();

            var allItems = HonorSvc.HonorList.Where(h => !(h is LimitHonorModel)).SelectMany(h => h.Items).Select(p => p.Name);
            if (items.Count != allItems.Count())
            {
                MsgSender.PushMsg(MsgDTO, "你尚未集齐所有非限定物品！", true);
                return false;
            }

            var response = WaiterSvc.WaitForInformation(MsgDTO, $"请输入想获取的荣誉称号名称(不能超过{RebornHonorLimit}个字)",
                info => info.FromQQ == MsgDTO.FromQQ && info.FromGroup == MsgDTO.FromGroup && info.Msg != null && info.Msg.Length <= RebornHonorLimit, 10);
            if (response == null)
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            var honorName = response.Msg;
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (osPerson.HonorNames.Contains(honorName))
            {
                MsgSender.PushMsg(MsgDTO, "你已经获取了该荣誉称号，操作取消！");
                return false;
            }

            osPerson.Level++;
            osPerson.HonorNames.Add(honorName);

            foreach (var honor in normalHonors.Select(p => p.Key))
            {
                TransHelper.SellHonorToShop(itemColl, honor, osPerson);
            }

            itemColl.Update();
            osPerson.Update();

            MsgSender.PushMsg(MsgDTO, $"恭喜【{honorName}】，转生成功！");
            return true;
        }
    }
}
