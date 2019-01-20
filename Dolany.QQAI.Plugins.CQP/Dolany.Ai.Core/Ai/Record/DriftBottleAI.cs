namespace Dolany.Ai.Core.Ai.Record
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Dolany.Ai.Common;
    using Dolany.Ai.Core.Base;
    using Dolany.Ai.Core.Cache;
    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Model;
    using Dolany.Database;
    using Dolany.Database.Ai;
    using Dolany.Database.Sqlite;
    using Dolany.Database.Sqlite.Model;

    [AI(
        Name = nameof(DriftBottleAI),
        Description = "AI for drift bottle.",
        IsAvailable = true,
        PriorityLevel = 10)]
    public class DriftBottleAI : AIBase
    {
        private readonly List<DriftBottleItemModel> Items = new List<DriftBottleItemModel>();

        private int SumRate;

        private const int FishingLimit = 2;

        private const int ThrowLimit = 2;

        private const int ItemRate = 70;

        public override void Initialization()
        {
            LoadItems();
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
                        new DriftBottleItemModel { Name = strs[0], Description = strs[1], Rate = int.Parse(strs[2]) });
                }
            }

            this.SumRate = this.Items.Sum(i => i.Rate);
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
            var cache = SqliteCacheService.Get<DriftBottleFishingCache>(key);
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

            SqliteCacheService.Cache(
                key,
                new DriftBottleFishingCache { QQNum = MsgDTO.FromQQ, Count = count + 1 },
                CommonUtil.UntilTommorow());
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

            var count = 0;

            var key = $"ThrowBottle-{MsgDTO.FromQQ}";
            var cache = SqliteCacheService.Get<DriftBottleThrowCache>(key);
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

            SqliteCacheService.Cache(
                key,
                new DriftBottleThrowCache { QQNum = MsgDTO.FromQQ, Count = count + 1 },
                CommonUtil.UntilTommorow());

            MsgSender.Instance.PushMsg(MsgDTO, "漂流瓶已随波而去，最终将会漂到哪里呢~");
        }

        private void FishItem(MsgInformationEx MsgDTO)
        {
            var item = LocalateItem(Utility.RandInt(this.SumRate));
            PrintItem(MsgDTO, item);
        }

        private void PrintItem(MsgInformationEx MsgDTO, DriftBottleItemModel item)
        {
            var msg =
                $"你捞到了 {item.Name} \r" + 
                $"    {item.Description} \r" + 
                $" 稀有率为 {Math.Round(item.Rate * 1.0 / this.SumRate * 100, 2)}%";

            MsgSender.Instance.PushMsg(MsgDTO, msg, true);
        }

        private void PrintBottle(MsgInformationEx MsgDTO, DriftBottleRecord record)
        {
            var msg = $"你捞到了一个漂流瓶 \r    {record.Content}   by 陌生人";
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
