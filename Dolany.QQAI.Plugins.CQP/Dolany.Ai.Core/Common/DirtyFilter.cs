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
        private static readonly List<string> WordList = MongoService<DirtyWord>.Get().Select(d => d.Content).ToList();

        private const int MaxTolerateCount = 10;

        public static bool Filter(long GroupNum, long QQNum, string msg)
        {
            if (!IsDirtyWord(msg))
            {
                return true;
            }
            AddInBlackList(GroupNum, QQNum);
            return false;
        }

        private static void AddInBlackList(long GroupNum, long QQNum)
        {
            var query = MongoService<BlackList>.Get(b => b.QQNum == QQNum);
            if (!query.IsNullOrEmpty())
            {
                var black = query.First();
                black.BlackCount++;
                black.UpdateTime = DateTime.Now;

                MongoService<BlackList>.Update(black);
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

        public static bool IsInBlackList(long fromQQ)
        {
            var query = MongoService<BlackList>.Get(b => b.QQNum == fromQQ);
            if (query.IsNullOrEmpty())
            {
                return false;
            }

            return query.First().BlackCount >= MaxTolerateCount;
        }

        private static bool IsDirtyWord(string msg)
        {
            return WordList.Any(msg.Contains);
        }
    }
}
