namespace Dolany.Ai.Core.Ai.Record
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Dolany.Ai.Common;
    using Base;
    using Cache;
    using Common;
    using Model;
    using Database;
    using Dolany.Database.Ai;
    using Database.Sqlite;
    using Dolany.Database.Sqlite.Model;

    [AI(
        Name = nameof(DriftBottleAI),
        Description = "AI for drift bottle.",
        IsAvailable = true,
        PriorityLevel = 10)]
    public class DriftBottleAI : AIBase
    {
        private readonly List<DriftBottleItemModel> Items = new List<DriftBottleItemModel>();

        private Dictionary<string, string[]> HonorDic = new Dictionary<string, string[]>();

        private int SumRate;

        private const int FishingLimit = 2;

        private const int ThrowLimit = 2;

        private const int ItemRate = 60;

        public override void Initialization()
        {
            LoadItems();
            LoadHonors();
        }

        private void LoadItems()
        {
            using (var reader = new StreamReader(new FileInfo("DriftBottleItems.ini").FullName))
            {
                string line;
                while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                {
                    var strs = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    this.Items.Add(
                        new DriftBottleItemModel
                            {
                                Name = strs[0], Description = strs[1], Rate = int.Parse(strs[2]),
                                Honor = strs[3]
                            });
                }
            }

            this.SumRate = this.Items.Sum(i => i.Rate);
        }

        private void LoadHonors()
        {
            HonorDic = Items.GroupBy(i => i.Honor).ToDictionary(i => i.Key, i => i.Select(p => p.Name).Distinct().ToArray());
        }

        [EnterCommand(
            Command = "捞瓶子",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "捞一个漂流瓶",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "漂流瓶功能",
            IsPrivateAvailable = true)]
        public void FishingBottle(MsgInformationEx MsgDTO, object[] param)
        {
            var count = 0;

            var key = $"DriftBottle-{MsgDTO.FromQQ}";
            var cache = SCacheService.Get<DriftBottleFishingCache>(key);
            if (cache != null && cache.Count >= FishingLimit)
            {
                MsgSender.Instance.PushMsg(MsgDTO, $"每天只能捞{FishingLimit}次瓶子噢~", true);
                return;
            }

            if (cache != null)
            {
                count = cache.Count;
            }

            if (Utility.RandInt(100) >= ItemRate)
            {
                var query = MongoService<DriftBottleRecord>.Get(
                    r => r.FromQQ != MsgDTO.FromQQ && r.FromGroup != MsgDTO.FromGroup && !r.ReceivedQQ.HasValue);
                if (!query.IsNullOrEmpty())
                {
                    var qcount = query.Count;
                    var bottle = query[Utility.RandInt(qcount)];
                    PrintBottle(MsgDTO, bottle);

                    bottle.ReceivedGroup = MsgDTO.FromGroup;
                    bottle.ReceivedQQ = MsgDTO.FromQQ;
                    bottle.ReceivedTime = DateTime.Now;

                    MongoService<DriftBottleRecord>.Update(bottle);
                }
                else
                {
                    FishItem(MsgDTO);
                }
            }
            else
            {
                FishItem(MsgDTO);
            }

            SCacheService.Cache(key, new DriftBottleFishingCache { QQNum = MsgDTO.FromQQ, Count = count + 1 });
        }

        [EnterCommand(
            Command = "扔瓶子",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "扔一个漂流瓶",
            Syntax = "[漂流瓶内容]",
            SyntaxChecker = "Any",
            Tag = "漂流瓶功能",
            IsPrivateAvailable = true)]
        public void ThrowBottle(MsgInformationEx MsgDTO, object[] param)
        {
            var content = param[0] as string;

            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            var count = 0;

            var key = $"ThrowBottle-{MsgDTO.FromQQ}";
            var cache = SCacheService.Get<DriftBottleThrowCache>(key);
            if (cache != null && cache.Count >= ThrowLimit)
            {
                MsgSender.Instance.PushMsg(MsgDTO, $"每天只能扔{FishingLimit}次瓶子噢~", true);
                return;
            }

            if (cache != null)
            {
                count = cache.Count;
            }

            MongoService<DriftBottleRecord>.Insert(
                new DriftBottleRecord
                    {
                        FromGroup = MsgDTO.FromGroup, FromQQ = MsgDTO.FromQQ, Content = content, SendTime = DateTime.Now
                    });

            SCacheService.Cache(key, new DriftBottleThrowCache { QQNum = MsgDTO.FromQQ, Count = count + 1 });

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

            var itemMsgs = query.ItemCount.Select(ic => $"{ic.Name}*{ic.Count}");
            var msg = $"你收集到的物品有：{itemMsgs}";
            MsgSender.Instance.PushMsg(MsgDTO, msg, true);
        }

        private void FishItem(MsgInformationEx MsgDTO)
        {
            var item = LocalateItem(Utility.RandInt(this.SumRate));
            int count;
            var honor = string.Empty;
            var query = MongoService<DriftItemRecord>.Get(r => r.QQNum == MsgDTO.FromQQ).FirstOrDefault();
            if (query == null || query.ItemCount.All(i => i.Name != item.Name))
            {
                MongoService<DriftItemRecord>.Insert(
                    new DriftItemRecord { QQNum = MsgDTO.FromQQ, ItemCount = new[] { new DriftItemCountRecord { Count = 1, Name = item.Name } } });

                count = 1;
                honor = CheckHonor(query, item);
            }
            else
            {
                var ic = query.ItemCount.First(i => i.Name == item.Name);
                ic.Count++;
                MongoService<DriftItemRecord>.Update(query);

                count = ic.Count;
            }

            PrintItem(MsgDTO, item, count, honor);
        }

        private string CheckHonor(DriftItemRecord record, DriftBottleItemModel item)
        {
            var honorName = item.Honor;
            if (record.HonorList.Contains(honorName))
            {
                return string.Empty;
            }

            var allNeedItems = HonorDic[honorName];
            if (!allNeedItems.All(ani => record.ItemCount.Any(ic => ic.Name == ani)))
            {
                return string.Empty;
            }

            record.HonorList = record.HonorList.Append(honorName);
            MongoService<DriftItemRecord>.Update(record);
            return honorName;
        }

        private void PrintItem(MsgInformationEx MsgDTO, DriftBottleItemModel item, int count, string honor)
        {
            var msg =
                $"你捞到了 {item.Name} \r" +
                $"    {item.Description} \r" +
                $" 稀有率为 {Math.Round(item.Rate * 1.0 / this.SumRate * 100, 2)}%\r" +
                $"你总共拥有该物品 {count}个";

            MsgSender.Instance.PushMsg(MsgDTO, msg, true);

            if (string.IsNullOrEmpty(honor))
            {
                return;
            }

            msg = $"恭喜你解锁了成就 {honor}! (集齐物品：{string.Join("，", HonorDic[honor])})";
            MsgSender.Instance.PushMsg(MsgDTO, msg, true);
        }

        private void PrintBottle(MsgInformationEx MsgDTO, DriftBottleRecord record)
        {
            var msg = $"你捞到了一个漂流瓶 \r    {record.Content}\r   by 陌生人";
            MsgSender.Instance.PushMsg(MsgDTO, msg);
        }

        private DriftBottleItemModel LocalateItem(int index)
        {
            var totalSum = 0;
            foreach (var item in this.Items)
            {
                if (index < totalSum + item.Rate)
                {
                    return item;
                }

                totalSum += item.Rate;
            }

            return this.Items.First();
        }
    }
}
