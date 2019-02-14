﻿using Dolany.Game.OnlineStore;

namespace Dolany.Ai.Core.Ai.Record
{
    using System;
    using System.Linq;

    using Dolany.Ai.Common;
    using Base;
    using Cache;
    using Model;
    using Database;
    using Dolany.Database.Ai;

    [AI(
        Name = "漂流瓶",
        Description = "AI for drift bottle.",
        Enable = true,
        PriorityLevel = 10,
        NeedManulOpen = true)]
    public class DriftBottleAI : AIBase
    {
        private const int ItemRate = 60;

        [EnterCommand(
            Command = "捞瓶子",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "捞一个漂流瓶",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "漂流瓶功能",
            IsPrivateAvailable = true,
            DailyLimit = 1,
            TestingDailyLimit = 3)]
        public void FishingBottle(MsgInformationEx MsgDTO, object[] param)
        {
            if (CommonUtil.RandInt(100) < ItemRate)
            {
                FishItem(MsgDTO);
                return;
            }

            var query = MongoService<DriftBottleRecord>.Get(r => r.FromQQ != MsgDTO.FromQQ && r.FromGroup != MsgDTO.FromGroup && !r.ReceivedQQ.HasValue);
            if (query.IsNullOrEmpty())
            {
                FishItem(MsgDTO);
                return;
            }

            var qcount = query.Count;
            var bottle = query[CommonUtil.RandInt(qcount)];
            PrintBottle(MsgDTO, bottle);

            bottle.ReceivedGroup = MsgDTO.FromGroup;
            bottle.ReceivedQQ = MsgDTO.FromQQ;
            bottle.ReceivedTime = DateTime.Now;

            MongoService<DriftBottleRecord>.Update(bottle);
        }

        [EnterCommand(
            Command = "扔瓶子",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "扔一个漂流瓶",
            Syntax = "[漂流瓶内容]",
            SyntaxChecker = "Any",
            Tag = "漂流瓶功能",
            IsPrivateAvailable = true,
            DailyLimit = 3)]
        public void ThrowBottle(MsgInformationEx MsgDTO, object[] param)
        {
            var content = param[0] as string;

            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            MongoService<DriftBottleRecord>.Insert(
                new DriftBottleRecord
                    {
                        FromGroup = MsgDTO.FromGroup, FromQQ = MsgDTO.FromQQ, Content = content, SendTime = DateTime.Now
                    });

            MsgSender.Instance.PushMsg(MsgDTO, "漂流瓶已随波而去，最终将会漂到哪里呢~");
        }

        [EnterCommand(Command = "我的物品",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看自己的物品",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "漂流瓶功能",
            IsPrivateAvailable = true)]
        public void MyItems(MsgInformationEx MsgDTO, object[] param)
        {
            var query = MongoService<DriftItemRecord>.Get(r => r.QQNum == MsgDTO.FromQQ).FirstOrDefault();
            if (query == null || !query.ItemCount.Any())
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你的背包空空如也~", true);
                return;
            }

            var itemMsgs = HonorHelper.Instance.GetOrderedItemsStr(query.ItemCount);
            var msg = $"你收集到的物品有：\r{itemMsgs}";
            MsgSender.Instance.PushMsg(MsgDTO, msg, true);
        }

        [EnterCommand(
            Command = "我的成就",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看自己的成就",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "漂流瓶功能",
            IsPrivateAvailable = true)]
        public void MyHonors(MsgInformationEx MsgDTO, object[] param)
        {
            var query = MongoService<DriftItemRecord>.Get(r => r.QQNum == MsgDTO.FromQQ).FirstOrDefault();
            if (query == null || query.HonorList == null || !query.HonorList.Any())
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你还没有获得任何成就，继续加油吧~", true);
                return;
            }

            var msg = $"你获得的成就有：{string.Join(",", query.HonorList)}";
            MsgSender.Instance.PushMsg(MsgDTO, msg, true);
        }

        [EnterCommand(
            Command = "我的物品",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "按成就名查看自己的物品",
            Syntax = "[成就名]",
            SyntaxChecker = "Word",
            Tag = "漂流瓶功能",
            IsPrivateAvailable = true)]
        public void MyItemsByHonor(MsgInformationEx MsgDTO, object[] param)
        {
            var honorName = param[0] as string;

            var honorItems = HonorHelper.Instance.FindHonorItems(honorName);
            if (honorItems.IsNullOrEmpty())
            {
                MsgSender.Instance.PushMsg(MsgDTO, "没有查找到该成就");
                return;
            }

            var query = MongoService<DriftItemRecord>.Get(r => r.QQNum == MsgDTO.FromQQ).FirstOrDefault();
            if (query == null || !query.ItemCount.Any())
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你的背包空空如也~", true);
                return;
            }

            var items = query.ItemCount.Where(ic => honorItems.Any(hi => hi.Name == ic.Name)).ToList();
            if (!items.Any())
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你没有属于该成就的物品", true);
                return;
            }

            var msg = $"属于该成就的你拥有的有：{string.Join(",", items.Select(ic => $"{ic.Name}*{ic.Count}"))}";
            MsgSender.Instance.PushMsg(MsgDTO, msg, true);
        }

        private void FishItem(MsgInformationEx MsgDTO)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (osPerson.CheckBuff("梅雨") && CommonUtil.RandInt(100) < 30)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "欸呀呀，捞瓶子失败了！", true);
                return;
            }

            var item = HonorHelper.Instance.RandItem();
            var (s, record) = ItemHelper.Instance.ItemIncome(MsgDTO.FromQQ, item.Name);
            var msg = $"你捞到了 {item.Name} \r" +
                      $"    {item.Description} \r" +
                      $"稀有率为 {HonorHelper.Instance.ItemRate(item)}%\r" +
                      $"售价为：{HonorHelper.Instance.GetItemPrice(item, MsgDTO.FromQQ)} 金币\r" +
                      $"你总共拥有该物品 {ItemHelper.Instance.ItemCount(record, item.Name)}个";

            MsgSender.Instance.PushMsg(MsgDTO, msg, true);

            if (!string.IsNullOrEmpty(s))
            {
                MsgSender.Instance.PushMsg(MsgDTO, s, true);
            }

            if (!osPerson.CheckBuff("钻石尘"))
            {
                return;
            }

            OSPerson.GoldConsume(MsgDTO.FromQQ, 40);
            MsgSender.Instance.PushMsg(MsgDTO, "欸呀呀，你丢失了40金币", true);
        }

        private void PrintBottle(MsgInformationEx MsgDTO, DriftBottleRecord record)
        {
            var msg = $"你捞到了一个漂流瓶 \r    {record.Content}\r   by 陌生人";
            MsgSender.Instance.PushMsg(MsgDTO, msg);
        }

        [EnterCommand(Command = "查看物品",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看某件物品的详情",
            Syntax = "[物品名称]",
            SyntaxChecker = "Word",
            Tag = "漂流瓶功能",
            IsPrivateAvailable = true)]
        public void ViewItem(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var item = HonorHelper.Instance.FindItem(name);
            if (item == null)
            {
                MsgSender.Instance.PushMsg(MsgDTO, $"未找到该物品：{name}");
                return;
            }

            var msg = $"物品名称：{item.Name}\r" +
                      $"物品描述：{item.Description}\r" +
                      $"稀有率：{HonorHelper.Instance.ItemRate(item)}%\r" +
                      $"可解锁成就：{item.Honor}\r" +
                      $"你拥有该物品：{ItemHelper.Instance.ItemCount(MsgDTO.FromQQ, item.Name)}";
            MsgSender.Instance.PushMsg(MsgDTO, msg);
        }

        [EnterCommand(
            Command = "查看成就",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看某个成就的详情",
            Syntax = "[成就名称]",
            SyntaxChecker = "Word",
            Tag = "漂流瓶功能",
            IsPrivateAvailable = true)]
        public void ViewHonor(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var honorItems = HonorHelper.Instance.FindHonorItems(name);
            if (honorItems.IsNullOrEmpty())
            {
                MsgSender.Instance.PushMsg(MsgDTO, $"未找到该成就：{name}");
                return;
            }

            var msg = $"解锁成就 {name} 需要集齐：{string.Join(",", honorItems.Select(h => $"{h.Name}({ItemHelper.Instance.ItemCount(MsgDTO.FromQQ, h.Name)})"))}";
            MsgSender.Instance.PushMsg(MsgDTO, msg);
        }
    }
}
