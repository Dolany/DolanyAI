﻿using System;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Database;
using Dolany.Database.Ai;
using Dolany.UtilityTool;
using Dolany.WorldLine.Standard.Ai.Game.Pet;
using Dolany.WorldLine.Standard.Ai.Vip;
using Dolany.WorldLine.Standard.OnlineStore;

namespace Dolany.WorldLine.Standard.Ai.Game.DriftBottle
{
    public class DriftBottleAI : AIBase
    {
        public override string AIName { get; set; } = "漂流瓶";

        public override string Description { get; set; } = "AI for drift bottle.";

        protected override CmdTagEnum DefaultTag { get; } = CmdTagEnum.漂流瓶功能;

        private const int ItemRate = 60;

        public HonorSvc HonorSvc { get; set; }

        [EnterCommand(ID = "DriftBottleAI_FishingBottle",
            Command = "捞瓶子",
            Description = "捞一个漂流瓶",
            DailyLimit = 1,
            TestingDailyLimit = 3)]
        public bool FishingBottle(MsgInformationEx MsgDTO, object[] param)
        {
            if (OSPersonBuff.CheckBuff(MsgDTO.FromQQ, "昙天"))
            {
                MsgSender.PushMsg(MsgDTO, "你当前无法捞瓶子！(昙天)");
                return false;
            }

            if (Rander.RandInt(100) < ItemRate)
            {
                FishItem(MsgDTO);
                return true;
            }

            var query = MongoService<DriftBottleRecord>.Get(r => r.FromQQ != MsgDTO.FromQQ && r.FromGroup != MsgDTO.FromGroup && !r.ReceivedQQ.HasValue);
            if (query.IsNullOrEmpty())
            {
                FishItem(MsgDTO);
                return true;
            }

            var bottle = query.RandElement();
            PrintBottle(MsgDTO, bottle);

            bottle.ReceivedGroup = MsgDTO.FromGroup;
            bottle.ReceivedQQ = MsgDTO.FromQQ;
            bottle.ReceivedTime = DateTime.Now;

            MongoService<DriftBottleRecord>.Update(bottle);
            return true;
        }

        [EnterCommand(ID = "DriftBottleAI_ThrowBottle",
            Command = "扔瓶子",
            Description = "扔一个漂流瓶",
            SyntaxHint = "[漂流瓶内容]",
            SyntaxChecker = "Any",
            IsPrivateAvailable = true,
            DailyLimit = 3,
            TestingDailyLimit = 3)]
        public bool ThrowBottle(MsgInformationEx MsgDTO, object[] param)
        {
            var content = param[0] as string;

            if (string.IsNullOrEmpty(content))
            {
                return false;
            }

            MongoService<DriftBottleRecord>.Insert(
                new DriftBottleRecord
                    {
                        FromGroup = MsgDTO.FromGroup, FromQQ = MsgDTO.FromQQ, Content = content, SendTime = DateTime.Now
                    });

            MsgSender.PushMsg(MsgDTO, "漂流瓶已随波而去，最终将会漂到哪里呢~");
            return true;
        }

        [EnterCommand(ID = "DriftBottleAI_MyItems",
            Command = "我的物品",
            Description = "查看自己的物品",
            IsPrivateAvailable = true)]
        public bool MyItems(MsgInformationEx MsgDTO, object[] param)
        {
            var query = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            if (query.TotalItemCount() == 0)
            {
                MsgSender.PushMsg(MsgDTO, "你的背包空空如也~", true);
                return false;
            }

            var itemMsgs = HonorSvc.GetOrderedItemsStr(query.HonorCollections.Where(p => p.Value.Type == HonorType.Normal).SelectMany(p => p.Value.Items)
                .ToDictionary(p => p.Key, p => p.Value));
            var msg = $"你收集到的物品有：\r\n{string.Join("\r\n", itemMsgs.Take(7))}";
            if (itemMsgs.Count > 7)
            {
                msg += $"\r\n当前显示第 1/{(itemMsgs.Count - 1) / 7 + 1}页，请使用 我的物品 [页码] 命令查看更多物品！";
            }
            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }

        [EnterCommand(ID = "DriftBottleAI_MyItemsByAttr",
            Command = "我的物品",
            Description = "查看自己指定特性的物品",
            SyntaxHint = "[特性名]",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true)]
        public bool MyItemsByAttr(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            if (!PetExtent.AllAttributes.Contains(name))
            {
                MsgSender.PushMsg(MsgDTO, "请输入正确的特性名！", true);
                return false;
            }

            var record = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            if (record.HonorCollections.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "你的背包空空如也~", true);
                return false;
            }

            var items = record.HonorCollections.Where(p => p.Value.Items != null).SelectMany(p => p.Value.Items
                .Where(item =>
                {
                    var itemModel = HonorSvc.FindItem(item.Key);
                    return itemModel.Attributes != null && itemModel.Attributes.Contains(name);
                })).ToList();
            if (!items.Any())
            {
                MsgSender.PushMsg(MsgDTO, "你没有该特性的物品！", true);
                return false;
            }

            var msg = $"你收集的物品中，【{name}】 特性的物品有：\r\n" +
                      string.Join(",", items.Select(item => $"{item.Key}*{item.Value}"));
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "DriftBottleAI_MyLimitItems",
            Command = "我的限定物品",
            Description = "查看自己的期间限定物品",
            IsPrivateAvailable = true)]
        public bool MyLimitItems(MsgInformationEx MsgDTO, object[] param)
        {
            var query = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            if (query.TotalItemCount() == 0)
            {
                MsgSender.PushMsg(MsgDTO, "你的背包空空如也~", true);
                return false;
            }

            var itemMsgs = HonorSvc.GetOrderedItemsStr(query.HonorCollections.Where(p => p.Value.Type == HonorType.Limit)
                .OrderByDescending(p => (HonorSvc.FindHonor(p.Key) as LimitHonorModel)?.SortKey).SelectMany(p => p.Value.Items)
                .ToDictionary(p => p.Key, p => p.Value));
            var msg = $"你收集到的限定物品有：\r\n{string.Join("\r\n", itemMsgs.Take(5))}";
            if (itemMsgs.Count > 5)
            {
                msg += $"\r\n当前显示第 1/{(itemMsgs.Count - 1) / 5 + 1}页，请使用 我的限定物品 [页码] 命令查看更多物品！";
            }
            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }

        [EnterCommand(ID = "DriftBottleAI_MyItems_Paged",
            Command = "我的物品",
            Description = "按页码查看自己的物品",
            SyntaxHint = "[页码]",
            SyntaxChecker = "Long",
            IsPrivateAvailable = true)]
        public bool MyItems_Paged(MsgInformationEx MsgDTO, object[] param)
        {
            var pageNo = (int) (long) param[0];

            var query = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            if (query.TotalItemCount() == 0)
            {
                MsgSender.PushMsg(MsgDTO, "你的背包空空如也~", true);
                return false;
            }

            var itemMsgs = HonorSvc.GetOrderedItemsStr(query.HonorCollections.Where(p => p.Value.Type == HonorType.Normal).SelectMany(p => p.Value.Items)
                .ToDictionary(p => p.Key, p => p.Value));
            var totalPageCount = (itemMsgs.Count - 1) / 7 + 1;
            if (pageNo <= 0 || pageNo > totalPageCount)
            {
                MsgSender.PushMsg(MsgDTO, "页码错误！", true);
                return false;
            }

            var msg = $"该页的物品有：\r\n{string.Join("\r\n", itemMsgs.Skip((pageNo - 1) * 7).Take(7))}";
            msg += $"\r\n当前显示第 {pageNo}/{(itemMsgs.Count - 1) / 7 + 1}页，请使用 我的物品 [页码] 命令查看更多物品！";
            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }

        [EnterCommand(ID = "DriftBottleAI_MyLimitItems",
            Command = "我的限定物品",
            Description = "按页码查看自己的期间限定物品",
            SyntaxHint = "[页码]",
            SyntaxChecker = "Long",
            IsPrivateAvailable = true)]
        public bool MyLimitItems_Paged(MsgInformationEx MsgDTO, object[] param)
        {
            var pageNo = (int) (long) param[0];

            var query = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            if (query.TotalItemCount() == 0)
            {
                MsgSender.PushMsg(MsgDTO, "你的背包空空如也~", true);
                return false;
            }

            var itemMsgs = HonorSvc.GetOrderedItemsStr(query.HonorCollections.Where(p => p.Value.Type == HonorType.Limit)
                .OrderByDescending(p => (HonorSvc.FindHonor(p.Key) as LimitHonorModel)?.SortKey).SelectMany(p => p.Value.Items)
                .ToDictionary(p => p.Key, p => p.Value));
            var totalPageCount = (itemMsgs.Count - 1) / 5 + 1;
            if (pageNo <= 0 || pageNo > totalPageCount)
            {
                MsgSender.PushMsg(MsgDTO, "页码错误！", true);
                return false;
            }

            var msg = $"该页的物品有：\r\n{string.Join("\r\n", itemMsgs.Skip((pageNo - 1) * 5).Take(5))}";
            if (itemMsgs.Count > 5)
            {
                msg += $"\r\n当前显示第 {pageNo}/{(itemMsgs.Count - 1) / 5 + 1}页，请使用 我的限定物品 [页码] 命令查看更多物品！";
            }
            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }

        [EnterCommand(ID = "DriftBottleAI_MyHonors",
            Command = "我的成就",
            Description = "查看自己的成就",
            IsPrivateAvailable = true)]
        public bool MyHonors(MsgInformationEx MsgDTO, object[] param)
        {
            var query = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            if (query.HonorList == null || query.HonorList.All(p => HonorSvc.IsLimitHonor(p)))
            {
                MsgSender.PushMsg(MsgDTO, "你还没有获得任何成就，继续加油吧~", true);
                return false;
            }

            var msg = $"你获得的普通成就有：{string.Join(",", query.HonorList.Where(p => !HonorSvc.IsLimitHonor(p)))}\r\n";
            msg += $"你获得的限定成就有：{string.Join(",", query.HonorList.Where(p => HonorSvc.IsLimitHonor(p)))}";
            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }

        private void FishItem(MsgInformationEx MsgDTO)
        {
            if (OSPersonBuff.CheckBuff(MsgDTO.FromQQ, "梅雨") && Rander.RandInt(100) < 30)
            {
                MsgSender.PushMsg(MsgDTO, "欸呀呀，捞瓶子失败了！(梅雨)", true);
                return;
            }

            var item = HonorSvc.RandItem();
            var record = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var honorName = HonorSvc.FindHonorName(item.Name);

            var count = 1;
            var vipArmers = VipArmerRecord.Get(MsgDTO.FromQQ);
            if (vipArmers.CheckArmer("安妮的镜子"))
            {
                count = 2;
            }
            vipArmers.Armers.Remove(p => p.Name == "安妮的镜子");
            vipArmers.Update();

            DriftBottleAnalyzeRecord.Record(item.Name, count);

            var s = record.ItemIncome(item.Name, count);
            var msg = "你捞到了 \r\n" +
                      $"{(string.IsNullOrEmpty(item.PicPath) ? string.Empty : $"{CodeApi.Code_Image_Relational(item.PicPath)}\r\n")}" +
                      $"{item.Name}{(count > 1 ? $"*{count}" : string.Empty)} \r\n" +
                      $"    {item.Description} \r\n" +
                      $"稀有率为 {HonorSvc.ItemRate(item)}%\r\n" +
                      $"售价为：{item.Price} 金币\r\n" +
                      $"特性：{(item.Attributes == null ? "无" : string.Join(",", item.Attributes))}\r\n" +
                      $"你总共拥有该物品 {record.HonorCollections[honorName].Items[item.Name]}个";

            if (!string.IsNullOrEmpty(s))
            {
                msg += $"\r\n{s}";
            }

            if (OSPersonBuff.CheckBuff(MsgDTO.FromQQ, "钻石尘") && Rander.RandInt(100) < 50)
            {
                OSPerson.GoldConsume(MsgDTO.FromQQ, 40);
                msg += "\r\n欸呀呀，你丢失了40金币(钻石尘)";
            }

            MsgSender.PushMsg(MsgDTO, msg, true);
        }

        private static void PrintBottle(MsgInformationEx MsgDTO, DriftBottleRecord record)
        {
            var msg = $"你捞到了一个漂流瓶 \r\n    “{record.Content}”\r\n   by 陌生人";
            MsgSender.PushMsg(MsgDTO, msg);
        }

        [EnterCommand(ID = "DriftBottleAI_ViewItem",
            Command = "查看物品",
            Description = "查看某件物品的详情",
            SyntaxHint = "[物品名称]",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true)]
        public bool ViewItem(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var item = HonorSvc.FindItem(name);
            if (item == null)
            {
                MsgSender.PushMsg(MsgDTO, $"未找到该物品：{name}");
                return false;
            }

            var record = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var msg = $"{(string.IsNullOrEmpty(item.PicPath) ? string.Empty : $"{CodeApi.Code_Image_Relational(item.PicPath)}\r\n")}" +
                      $"{item.Name}\r\n" +
                      $"    {item.Description}\r\n" +
                      $"稀有率：{HonorSvc.ItemRate(item)}%\r\n" +
                      $"价格：{HonorSvc.GetItemPrice(item, MsgDTO.FromQQ).CurencyFormat()}\r\n" +
                      $"特性：{(item.Attributes == null ? "无" : string.Join(",", item.Attributes))}\r\n" +
                      $"可解锁成就：{HonorSvc.FindHonorFullName(item.Name)}\r\n" +
                      $"你拥有该物品：{record.GetCount(item.Name)}个";
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "DriftBottleAI_ViewHonor",
            Command = "查看成就",
            Description = "查看某个成就的详情",
            SyntaxHint = "[成就名称]",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true)]
        public bool ViewHonor(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var honor = HonorSvc.FindHonor(name);
            if (honor == null)
            {
                MsgSender.PushMsg(MsgDTO, $"未找到该成就：{name}");
                return false;
            }

            var record = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var items = honor.Items.Select(h => $"{h.Name}*{record.GetCount(h.Name)}");
            var itemsMsg = string.Join(",", items);
            var msg = $"解锁成就 【{honor.FullName}】 需要集齐：{itemsMsg}";
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "DriftBottleAI_LimitItemReport",
            Command = "限定物品收集报告",
            Description = "获取当月限定物品的收集报告",
            IsPrivateAvailable = true)]
        public bool LimitItemReport(MsgInformationEx MsgDTO, object[] param)
        {
            var LimitHonor = HonorSvc.HonorList.First(h => h is LimitHonorModel model && model.IsCurLimit);
            var LimitItemsNames = LimitHonor.Items.Select(i => i.Name);

            var allRecord = MongoService<ItemCollectionRecord>.Get(r => r.HonorCollections.ContainsKey(LimitHonor.Name));
            var itemDic = LimitItemsNames.ToDictionary(p => p, p => 0);

            foreach (var colle in allRecord.Select(record => record.HonorCollections[LimitHonor.Name]))
            {
                foreach (var (key, value) in colle.Items)
                {
                    itemDic[key] += value;
                }
            }

            var honorCount = allRecord.Count(r => r.HonorList != null && r.HonorList.Contains(LimitHonor.Name));
            var msg = $"限定物品收集情况：\r\n{string.Join("\r\n", itemDic.Select(p => $"{p.Key}:{p.Value}"))}\r\n";
            msg += $"共有 {honorCount} 人达成了本月限定成就";

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "DriftBottleAI_TodayDriftAnalyze",
            Command = "今日捞瓶子统计",
            Description = "查看今日捞瓶子情况统计",
            IsPrivateAvailable = true)]
        public bool TodayDriftAnalyze(MsgInformationEx MsgDTO, object[] param)
        {
            var todayRec = DriftBottleAnalyzeRecord.GetToday();
            if (todayRec.ItemDic.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "尚未有人捞到瓶子！");
                return false;
            }

            var modelDic = todayRec.ItemDic.ToDictionary(p => HonorSvc.FindItem(p.Key), p => p.Value);
            var msg = "今日捞瓶子统计\r\n";
            msg += $"总次数：{todayRec.ItemDic.Sum(p => p.Value)}\r\n";
            msg += $"总价值：{modelDic.Sum(p => p.Key.Price * p.Value)}";

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "DriftBottleAI_ItemCount",
            Command = "物品统计",
            Description = "查看指定物品的数量",
            SyntaxHint = "[物品名]",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true)]
        public bool ItemCount(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;

            if (HonorSvc.FindItem(name) == null)
            {
                MsgSender.PushMsg(MsgDTO, "未查找到相关物品！");
                return false;
            }

            var honor = HonorSvc.FindHonorName(name);

            var recs = MongoService<ItemCollectionRecord>.Get(p =>
                p.HonorCollections != null && p.HonorCollections.ContainsKey(honor) && p.HonorCollections[honor].Items.ContainsKey(name));

            var count = recs.Sum(r => r.HonorCollections[honor].Items[name]);
            MsgSender.PushMsg(MsgDTO, $"当前{name}被收集了{count}个");
            return true;
        }

        [EnterCommand(ID = "DriftBottleAI_MyLackItems",
            Command = "我缺少的物品",
            Description = "查看自己缺少的物品（仅当缺少的物品少于20件时显示详情信息）",
            IsPrivateAvailable = true)]
        public bool MyLackItems(MsgInformationEx MsgDTO, object[] param)
        {
            var itemColle = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var allColleItems = itemColle.HonorCollections.SelectMany(p => p.Value.Items).Select(p => p.Key);
            var allItems = HonorSvc.HonorList.Where(h => !h.IsLimit).SelectMany(h => h.Items);

            var lackItems = allItems.Where(p => !allColleItems.Contains(p.Name)).ToList();
            if (lackItems.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "恭喜你已经集齐了所有物品，快使用【灵魂转生】命令来升级吧！");
                return false;
            }

            var msg = $"你总共缺少{lackItems.Count}件物品\r\n";
            if (lackItems.Count <= 20)
            {
                msg += string.Join(",", lackItems.Select(p => p.Name));
            }
            else
            {
                msg += "你缺少的物品太多啦！这里就不一一列举了！";
            }

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }
    }
}
