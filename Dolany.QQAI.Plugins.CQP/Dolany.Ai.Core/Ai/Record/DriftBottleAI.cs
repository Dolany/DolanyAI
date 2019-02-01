using Dolany.Game.OnlineStore;

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
        Name = nameof(DriftBottleAI),
        Description = "AI for drift bottle.",
        Enable = true,
        PriorityLevel = 10)]
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

            var itemMsgs = query.ItemCount.Take(20).Select(ic => $"{ic.Name}*{ic.Count}");
            var msg = $"你收集到的物品有：{string.Join(",", itemMsgs)}";
            if (query.ItemCount.Count() > 20)
            {
                var pageCount = (query.ItemCount.Count() - 1) / 20 + 1;
                msg += $"\r(当前第1/{pageCount}页，请使用 我的物品 [页码] 来查看更多物品)";
            }
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

        [EnterCommand(Command = "我的物品",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "分页查看自己的物品(每页20个)",
            Syntax = "[页码]",
            SyntaxChecker = "Long",
            Tag = "漂流瓶功能",
            IsPrivateAvailable = true)]
        public void MyItemsPaged(MsgInformationEx MsgDTO, object[] param)
        {
            var pageIdx = (long)param[0];
            if (pageIdx <= 0)
            {
                return;
            }

            var query = MongoService<DriftItemRecord>.Get(r => r.QQNum == MsgDTO.FromQQ).FirstOrDefault();
            if (query == null || !query.ItemCount.Any())
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你的背包空空如也~", true);
                return;
            }
            var pageCount = (query.ItemCount.Count() - 1) / 20 + 1;
            if (pageIdx > pageCount)
            {
                MsgSender.Instance.PushMsg(MsgDTO, $"你的背包只有 {pageCount}页~", true);
                return;
            }

            var itemMsgs = query.ItemCount.Skip((int)(pageIdx - 1) * 20).Take(20).Select(ic => $"{ic.Name}*{ic.Count}");
            var msg = $"该页的物品有：{string.Join(",", itemMsgs)}";
            if (pageCount > 1)
            {
                msg += $"\r(当前第{pageIdx}/{pageCount}页，请使用 我的物品 [页码] 来查看更多物品)";
            }
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
            var item = HonorHelper.Instance.RandItem();
            var itemMsg = ItemHelper.Instance.ItemIncome(MsgDTO.FromQQ, item.Name);
            var msg = $"你捞到了 {item.Name} \r" + 
                      $"    {item.Description} \r" + 
                      $" 稀有率为 {HonorHelper.Instance.ItemRate(item)}%\r" + 
                      $"你总共拥有该物品 {ItemHelper.Instance.ItemCount(MsgDTO.FromQQ, item.Name)}个";

            MsgSender.Instance.PushMsg(MsgDTO, msg, true);

            if (!string.IsNullOrEmpty(itemMsg))
            {
                MsgSender.Instance.PushMsg(MsgDTO, itemMsg, true);
            }
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

            var msg = $"物品名称：{item.Name}\r物品描述：{item.Description}\r稀有率：{HonorHelper.Instance.ItemRate(item)}%\r可解锁成就：{item.Honor}";
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

            var msg = $"解锁成就 {name} 需要集齐：{string.Join(",", honorItems.Select(h => h.Name))}";
            MsgSender.Instance.PushMsg(MsgDTO, msg);
        }
    }
}
