using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Ai.Game.Advanture;
using Dolany.Ai.Core.Ai.Game.Gift;
using Dolany.Ai.Core.Ai.Game.Pet;
using Dolany.Ai.Core.API;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.OnlineStore;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Ai.Game.Shopping
{
    public class ShoppingAI : AIBase
    {
        public override string AIName { get; set; } = "商店";

        public override string Description { get; set; } = "AI for Shopping.";

        public override int PriorityLevel { get; set; } = 10;

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
                IsLimit = HonorHelper.Instance.IsLimitItem(p.Key),
                Price = HonorHelper.GetItemPrice(HonorHelper.Instance.FindItem(p.Key), MsgDTO.FromQQ)
            }).ToList();
            var msg = $"你即将贩卖{ictm.Sum(i => i.Count)}件物品，" +
                      $"其中有{ictm.Count(i => i.IsLimit)}件限定物品，" +
                      $"共价值{ictm.Sum(p => p.Price)}金币，是否继续？";
            if (!Waiter.Instance.WaitForConfirm(MsgDTO, msg))
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
            osPerson.Golds += ictm.Sum(p => p.Price);
            osPerson.Update();

            MsgSender.PushMsg(MsgDTO, $"贩卖成功，你当前拥有{osPerson.Golds}金币！");
            return true;
        }

        private static void SellItem(MsgInformationEx MsgDTO, DriftBottleItemModel item)
        {
            var record = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            if (!record.CheckItem(item.Name))
            {
                MsgSender.PushMsg(MsgDTO, "你的背包里没有该物品！");
                return;
            }

            var price = HonorHelper.GetItemPrice(item, MsgDTO.FromQQ);
            var msg = $"贩卖此物品将获得 {price} 金币，是否确认贩卖？";
            if (!Waiter.Instance.WaitForConfirm(MsgDTO, msg))
            {
                MsgSender.PushMsg(MsgDTO, "交易取消！");
                return;
            }

            var golds = TransHelper.SellItemToShop(MsgDTO.FromQQ, item.Name);
            MsgSender.PushMsg(MsgDTO, $"贩卖成功！你当前拥有金币 {golds}");
        }

        private static void SellHonor(MsgInformationEx MsgDTO, string honorName)
        {
            var query = MongoService<ItemCollectionRecord>.GetOnly(r => r.QQNum == MsgDTO.FromQQ);
            if (query == null || query.HonorCollections.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "你尚未拥有任何物品！");
                return;
            }

            var items = HonorHelper.Instance.FindHonor(honorName).Items;
            var honorCollection = query.HonorCollections;
            if (!honorCollection.ContainsKey(honorName) || honorCollection[honorName].Items.Count < items.Count)
            {
                MsgSender.PushMsg(MsgDTO, "你尚未集齐该成就下的所有物品！");
                return;
            }

            var price = HonorHelper.Instance.GetHonorPrice(honorName, MsgDTO.FromQQ);
            var msg = $"贩卖此成就将获得 {price} 金币，是否确认贩卖？";
            if (!Waiter.Instance.WaitForConfirm(MsgDTO, msg))
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
            var record = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var itemsStr = string.Join("\r", sellItems.Select(si =>
                $"{si.Name}({HonorHelper.Instance.FindHonorFullName(si.Name)})({record.GetCount(si.Name)})({si.Attr})：{si.Price}{Emoji.钱袋}"));

            var msg = $"今日售卖的商品：\r{itemsStr}\r你当前持有 {golds}{Emoji.钱袋}";
            MsgSender.PushMsg(MsgDTO, msg);

            return true;
        }

        [EnterCommand(ID = "ShoppingAI_Buy",
            Command = "购买 购买物品",
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
            if (!Waiter.Instance.WaitForConfirm_Gold(MsgDTO, price))
            {
                MsgSender.PushMsg(MsgDTO, "交易取消！");
                return false;
            }

            var record = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var incomeMsg = record.ItemIncome(sellItem.Name);

            OSPerson.GoldConsume(osPerson.QQNum, price);

            MsgSender.PushMsg(MsgDTO, $"{incomeMsg}\r购买成功！你当前剩余的金币为 {osPerson.Golds - sellItem.Price}");
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

                var aimRecord = ItemCollectionRecord.Get(aimQQ);
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

                var sourceRecord = ItemCollectionRecord.Get(MsgDTO.FromQQ);
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
            var itemRecord = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var advPlayer = AdvPlayer.GetPlayer(MsgDTO.FromQQ);
            var glamourRecord = GlamourRecord.Get(MsgDTO.FromGroup, MsgDTO.FromQQ);

            var normalHonors = itemRecord.HonorCollections.Where(h => h.Value.Type == HonorType.Normal).ToList();
            var items = normalHonors.Select(p => p.Value).SelectMany(h => h.Items.Keys).ToList();

            var allItems = HonorHelper.Instance.HonorList.Where(h => !(h is LimitHonorModel)).SelectMany(h => h.Items).Select(p => p.Name).ToArray();

            var msg = $"等级：{osPerson.EmojiLevel}\r" +
                      $"经验值：{items.Count}/{allItems.Length}{(items.Count == allItems.Length ? "(可转生)" : string.Empty)}\r" +
                      $"{(osPerson.HonorNames.IsNullOrEmpty() ? "" : string.Join("", osPerson.HonorNames.Select(h => $"【{h}】"))) + "\r"}" +
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

            var sourceRecord = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            if (!sourceRecord.CheckItem(name))
            {
                MsgSender.PushMsg(MsgDTO, "你没有此物品", true);
                return false;
            }

            var itemModel = HonorHelper.Instance.FindItem(name);
            var price = HonorHelper.GetItemPrice(itemModel, MsgDTO.FromQQ) * 5 / 100;
            if (!Waiter.Instance.WaitForConfirm_Gold(MsgDTO, price))
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
                res += $"\r{msg}";
            }
            MsgSender.PushMsg(MsgDTO, res);

            return true;
        }

        [EnterCommand(ID = "ShoppingAI_AssertCalculate",
            Command = "资产评估",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "评估自己的资产情况",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "商店功能",
            IsPrivateAvailable = true)]
        public bool AssertCalculate(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (osPerson.Golds < 50)
            {
                MsgSender.PushMsg(MsgDTO, $"你的金币余额不足({osPerson.Golds}/50)!");
                return false;
            }

            if (!Waiter.Instance.WaitForConfirm_Gold(MsgDTO, 50))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            osPerson.Golds -= 50;
            osPerson.Update();

            var resultDic = new Dictionary<string, int> {{"金币资产", osPerson.Golds}};

            var itemRecord = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            if (itemRecord.HonorCollections != null)
            {
                var itemAssert = 0;
                foreach (var (honorName, collection) in itemRecord.HonorCollections)
                {
                    var honorModel = HonorHelper.Instance.FindHonor(honorName);
                    var honorPrice = honorModel.Items.Sum(p => p.Price) * 3 / 2;
                    while (collection.Items != null && honorModel.Items.Count == collection.Items.Count)
                    {
                        itemAssert += honorPrice;

                        for (var i = 0; i < collection.Items.Count; i++)
                        {
                            collection.Items[collection.Items.Keys.ElementAt(i)]--;
                        }

                        collection.Items.Remove(p => p == 0);
                    }

                    itemAssert += collection.Items?.Sum(p => honorModel.Items.First(item => item.Name == p.Key).Price * p.Value) ?? 0;
                }

                resultDic.Add("物品资产", itemAssert);
            }

            if (osPerson.GiftDic != null)
            {
                var giftsMaterialDic = osPerson.GiftDic.SelectMany(p => GiftMgr.Instance[p.Key].MaterialDic);
                var giftAssert = giftsMaterialDic.Sum(g => HonorHelper.Instance.FindItem(g.Key).Price * g.Value);
                resultDic.Add("礼物资产", giftAssert);
            }

            var pet = PetRecord.Get(MsgDTO.FromQQ);
            if (pet.Level > 0 || pet.Exp > 0)
            {
                var petAssert = 0;
                for (var i = 1; i < pet.Level; i++)
                {
                    petAssert += PetLevelMgr.Instance[i].Exp * 10;
                }

                petAssert += pet.Exp * 10;
                resultDic.Add("宠物资产", petAssert);
            }

            var msg = "请查阅你的资产评估报告：\r" +
                      $"{string.Join("\r", resultDic.Select(rd => $"{rd.Key}:{rd.Value}{Emoji.钱袋}"))}" +
                      $"\r总资产:{resultDic.Sum(p => p.Value)}{Emoji.钱袋}";
            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }

        [EnterCommand(ID = "ShoppingAI_Reborn",
            Command = "灵魂转生",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "献祭所有物品，等级+1",
            Syntax = "",
            Tag = "商店功能",
            SyntaxChecker = "Empty",
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

            var allItems = HonorHelper.Instance.HonorList.Where(h => !(h is LimitHonorModel)).SelectMany(h => h.Items).Select(p => p.Name);
            if (items.Count != allItems.Count())
            {
                MsgSender.PushMsg(MsgDTO, "你尚未集齐所有非限定物品！", true);
                return false;
            }

            var response = Waiter.Instance.WaitForInformation(MsgDTO, "请输入想获取的荣誉称号名称(不能超过6个字)",
                info => info.FromQQ == MsgDTO.FromQQ && info.FromGroup == MsgDTO.FromGroup && info.Msg != null && info.Msg.Length <= 6, 10);
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

            foreach (var honor in normalHonors.Select(p => p.Key))
            {
                TransHelper.SellHonorToShop(itemColl, MsgDTO.FromQQ, honor);
            }

            osPerson.Level++;
            osPerson.HonorNames.Add(honorName);
            osPerson.Update();

            MsgSender.PushMsg(MsgDTO, "转生成功！");
            return true;
        }
    }
}
