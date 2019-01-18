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

        public bool Filter(long GroupNum, long QQNum, string msg)
        {
            if (!WordList.Any(msg.Contains))
            {
                return true;
            }
            AddInBlackList(GroupNum, QQNum);
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

            var mi = GroupNum == 0
                ? null
                : Utility.GetMemberInfo(new MsgInformationEx
                {
                    FromGroup = GroupNum,
                    FromQQ = QQNum
                });

            MongoService<BlackList>.Insert(new BlackList
            {
                Id = Guid.NewGuid().ToString(),
                BlackCount = 1,
                UpdateTime = DateTime.Now,
                QQNum = QQNum,
                NickName = mi == null ? string.Empty : mi.Nickname
            });
        }

        public bool IsInBlackList(long fromQQ)
        {
            return BlackList.Contains(fromQQ);
        }
    }
}
