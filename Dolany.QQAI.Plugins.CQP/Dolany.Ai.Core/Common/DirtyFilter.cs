namespace Dolany.Ai.Core.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Dolany.Ai.Common;
    using Database;
    using Dolany.Database.Ai;

    using Model;

    public class DirtyFilter
    {
        private readonly List<string> WordList = MongoService<DirtyWord>.Get().Select(d => d.Content).ToList();

        private readonly List<long> BlackList;

        public static DirtyFilter Instance { get; } = new DirtyFilter();

        private const int MaxTolerateCount = 10;

        private DirtyFilter()
        {
            BlackList = MongoService<BlackList>.Get(p => p.BlackCount >= MaxTolerateCount).Select(p => p.QQNum).ToList();
        }

        public bool Filter(MsgInformationEx MsgDTO)
        {
            if (BlackList.Contains(MsgDTO.FromQQ))
            {
                return false;
            }

            if (!WordList.Any(MsgDTO.Msg.Contains))
            {
                return true;
            }
            AddInBlackList(MsgDTO.FromGroup, MsgDTO.FromQQ);
            return false;
        }

        private void AddInBlackList(long GroupNum, long QQNum)
        {
            var query = MongoService<BlackList>.Get(b => b.QQNum == QQNum);
            if (!query.IsNullOrEmpty())
            {
                var black = query.First();
                black.BlackCount++;
                black.UpdateTime = DateTime.Now;

                MongoService<BlackList>.Update(black);

                if (black.BlackCount > MaxTolerateCount && !BlackList.Contains(QQNum))
                {
                    BlackList.Add(QQNum);
                }
                return;
            }

            MongoService<BlackList>.Insert(new BlackList
            {
                BlackCount = 1,
                UpdateTime = DateTime.Now,
                QQNum = QQNum
            });
        }

        public bool IsInBlackList(long fromQQ)
        {
            return BlackList.Contains(fromQQ);
        }
    }
}
