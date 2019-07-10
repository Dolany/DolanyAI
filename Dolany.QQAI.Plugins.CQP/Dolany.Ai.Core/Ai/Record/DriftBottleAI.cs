﻿using System;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.OnlineStore;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Ai.Record
{
    public class DriftBottleAI : AIBase
    {
        public override string AIName { get; set; } = "漂流瓶";

        public override string Description { get; set; } = "AI for drift bottle.";

        public override int PriorityLevel { get; set; } = 10;

        public override bool NeedManualOpeon { get; set; } = true;

        private const int ItemRate = 60;

        [EnterCommand(ID = "DriftBottleAI_FishingBottle",
            Command = "捞瓶子",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "捞一个漂流瓶",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "漂流瓶功能",
            IsPrivateAvailable = false,
            DailyLimit = 1,
            TestingDailyLimit = 3)]
        public bool FishingBottle(MsgInformationEx MsgDTO, object[] param)
        {
            if (OSPersonBuff.CheckBuff(MsgDTO.FromQQ, "昙天"))
            {
                MsgSender.PushMsg(MsgDTO, "你当前无法捞瓶子！(昙天)");
                return false;
            }

            if (CommonUtil.RandInt(100) < ItemRate)
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
            AuthorityLevel = AuthorityLevel.成员,
            Description = "扔一个漂流瓶",
            Syntax = "[漂流瓶内容]",
            SyntaxChecker = "Any",
            Tag = "漂流瓶功能",
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
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看自己的物品",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "漂流瓶功能",
            IsPrivateAvailable = true)]
        public bool MyItems(MsgInformationEx MsgDTO, object[] param)
        {
            var query = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            if (query.TotalItemCount() == 0)
            {
                MsgSender.PushMsg(MsgDTO, "你的背包空空如也~", true);
                return false;
            }

            var itemMsgs = HonorHelper.Instance.GetOrderedItemsStr(query.HonorCollections.Where(p => p.Value.Type == HonorType.Normal).SelectMany(p => p.Value.Items)
                .ToDictionary(p => p.Key, p => p.Value));
            var msg = $"你收集到的物品有：\r{string.Join("\r", itemMsgs.Take(7))}";
            if (itemMsgs.Count > 7)
            {
                msg += $"\r当前显示第 1/{(itemMsgs.Count - 1) / 7 + 1}页，请使用 我的物品 [页码] 命令查看更多物品！";
            }
            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }

        [EnterCommand(ID = "DriftBottleAI_MyLimitItems",
            Command = "我的限定物品",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看自己的期间限定物品",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "漂流瓶功能",
            IsPrivateAvailable = true)]
        public bool MyLimitItems(MsgInformationEx MsgDTO, object[] param)
        {
            var query = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            if (query.TotalItemCount() == 0)
            {
                MsgSender.PushMsg(MsgDTO, "你的背包空空如也~", true);
                return false;
            }

            var itemMsgs = HonorHelper.Instance.GetOrderedItemsStr(query.HonorCollections.Where(p => p.Value.Type == HonorType.Limit).SelectMany(p => p.Value.Items)
                .ToDictionary(p => p.Key, p => p.Value));
            var msg = $"你收集到的限定物品有：\r{string.Join("\r", itemMsgs.Take(7))}";
            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }

        [EnterCommand(ID = "DriftBottleAI_MyItems_Paged",
            Command = "我的物品",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "按页码查看自己的物品",
            Syntax = "[页码]",
            SyntaxChecker = "Long",
            Tag = "漂流瓶功能",
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

            var itemMsgs = HonorHelper.Instance.GetOrderedItemsStr(query.HonorCollections.Where(p => p.Value.Type == HonorType.Normal).SelectMany(p => p.Value.Items)
                .ToDictionary(p => p.Key, p => p.Value));
            var totalPageCount = (itemMsgs.Count - 1) / 7 + 1;
            if (pageNo <= 0 || pageNo > totalPageCount)
            {
                MsgSender.PushMsg(MsgDTO, "页码错误！", true);
                return false;
            }

            var msg = $"该页的物品有：\r{string.Join("\r", itemMsgs.Skip((pageNo - 1) * 7).Take(7))}";
            msg += $"\r当前显示第 {pageNo}/{(itemMsgs.Count - 1) / 7 + 1}页，请使用 我的物品 [页码] 命令查看更多物品！";
            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }

        [EnterCommand(ID = "DriftBottleAI_MyHonors",
            Command = "我的成就",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看自己的成就",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "漂流瓶功能",
            IsPrivateAvailable = true)]
        public bool MyHonors(MsgInformationEx MsgDTO, object[] param)
        {
            var query = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            if (query.HonorList == null || query.HonorList.All(p => HonorHelper.Instance.IsLimitHonor(p)))
            {
                MsgSender.PushMsg(MsgDTO, "你还没有获得任何成就，继续加油吧~", true);
                return false;
            }

            var msg = $"你获得的成就有：{string.Join(",", query.HonorList.Where(p => !HonorHelper.Instance.IsLimitHonor(p)))}";
            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }

        private static void FishItem(MsgInformationEx MsgDTO)
        {
            if (OSPersonBuff.CheckBuff(MsgDTO.FromQQ, "梅雨") && CommonUtil.RandInt(100) < 30)
            {
                MsgSender.PushMsg(MsgDTO, "欸呀呀，捞瓶子失败了！(梅雨)", true);
                return;
            }

            var item = HonorHelper.Instance.RandItem();
            var record = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var honorName = HonorHelper.Instance.FindHonorName(item.Name);

            var s = record.ItemIncome(item.Name);
            var msg = $"你捞到了 {item.Name} \r" +
                      $"    {item.Description} \r" +
                      $"稀有率为 {HonorHelper.Instance.ItemRate(item)}%\r" +
                      $"售价为：{HonorHelper.GetItemPrice(item, MsgDTO.FromQQ)} 金币\r" +
                      $"特性：{string.Join(",", item.Attributes)}\r" +
                      $"你总共拥有该物品 {record.HonorCollections[honorName].Items[item.Name]}个";

            if (!string.IsNullOrEmpty(s))
            {
                msg += $"\r{s}";
            }

            if (OSPersonBuff.CheckBuff(MsgDTO.FromQQ, "钻石尘") && CommonUtil.RandInt(100) < 50)
            {
                OSPerson.GoldConsume(MsgDTO.FromQQ, 40);
                msg += "\r欸呀呀，你丢失了40金币(钻石尘)";
            }

            MsgSender.PushMsg(MsgDTO, msg, true);
        }

        private static void PrintBottle(MsgInformationEx MsgDTO, DriftBottleRecord record)
        {
            var msg = $"你捞到了一个漂流瓶 \r    {record.Content}\r   by 陌生人";
            MsgSender.PushMsg(MsgDTO, msg);
        }

        [EnterCommand(ID = "DriftBottleAI_ViewItem",
            Command = "查看物品",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看某件物品的详情",
            Syntax = "[物品名称]",
            SyntaxChecker = "Word",
            Tag = "漂流瓶功能",
            IsPrivateAvailable = true)]
        public bool ViewItem(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var item = HonorHelper.Instance.FindItem(name);
            if (item == null)
            {
                MsgSender.PushMsg(MsgDTO, $"未找到该物品：{name}");
                return false;
            }

            var record = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var msg = $"物品名称：{item.Name}\r" +
                      $"物品描述：{item.Description}\r" +
                      $"稀有率：{HonorHelper.Instance.ItemRate(item)}%\r" +
                      $"价格：{HonorHelper.GetItemPrice(item, MsgDTO.FromQQ)}\r" +
                      $"特性：{string.Join(",", item.Attributes)}\r" +
                      $"可解锁成就：{HonorHelper.Instance.FindHonorFullName(item.Name)}\r" +
                      $"你拥有该物品：{record.GetCount(item.Name)}";
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "DriftBottleAI_ViewHonor",
            Command = "查看成就",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看某个成就的详情",
            Syntax = "[成就名称]",
            SyntaxChecker = "Word",
            Tag = "漂流瓶功能",
            IsPrivateAvailable = true)]
        public bool ViewHonor(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var honor = HonorHelper.Instance.FindHonor(name);
            if (honor == null)
            {
                MsgSender.PushMsg(MsgDTO, $"未找到该成就：{name}");
                return false;
            }

            var record = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var items = honor.Items.Select(h => $"{h.Name}*{record.GetCount(h.Name)}");
            var itemsMsg = string.Join(",", items);
            var msg = $"解锁成就 {honor.FullName} 需要集齐：{itemsMsg}";
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "DriftBottleAI_LimitItemReport",
            Command = "限定物品收集报告",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取当月限定物品的收集报告",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "漂流瓶功能",
            IsPrivateAvailable = true)]
        public bool LimitItemReport(MsgInformationEx MsgDTO, object[] param)
        {
            var LimitHonor = HonorHelper.Instance.HonorList.First(h =>
                h is LimitHonorModel model && model.Year == DateTime.Now.Year && model.Month == DateTime.Now.Month);
            var LimitItemsNames = LimitHonor.Items.Select(i => i.Name);

            var allRecord = MongoService<ItemCollectionRecord>.Get(r => r.HonorCollections.ContainsKey(LimitHonor.Name));
            var itemDic = LimitItemsNames.ToDictionary(p => p, p => 0);

            foreach (var record in allRecord)
            {
                var colle = record.HonorCollections[LimitHonor.Name];
                foreach (var (key, value) in colle.Items)
                {
                    itemDic[key] += value;
                }
            }

            var honorCount = allRecord.Count(r => r.HonorList != null && r.HonorList.Contains(LimitHonor.Name));
            var msg = $"限定物品收集情况：\r{string.Join("\r", itemDic.Select(p => $"{p.Key}:{p.Value}"))}\r";
            msg += $"共有 {honorCount} 人达成了本月限定成就";

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }
    }
}
