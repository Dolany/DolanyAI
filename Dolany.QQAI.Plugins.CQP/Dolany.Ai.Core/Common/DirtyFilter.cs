using System;
using System.Collections.Generic;

namespace Dolany.Ai.Core.Common
{
    using System.Linq;

    using Dolany.Ai.Core.Model;
    using Dolany.Database.Ai;

    public static class DirtyFilter
    {
        private static List<string> WordList;

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
            using (var db = new AIDatabase())
            {
                var query = db.BlackList.Where(b => b.QQNum == QQNum);
                if (!query.IsNullOrEmpty())
                {
                    var black = query.First();
                    black.BlackCount++;
                    black.UpdateTime = DateTime.Now;
                    db.SaveChanges();
                    return;
                }

                var mi = GroupNum == 0
                    ? null
                    : Utility.GetMemberInfo(new MsgInformationEx
                    {
                        FromGroup = GroupNum,
                        FromQQ = QQNum
                    });

                db.BlackList.Add(new BlackList
                {
                    Id = Guid.NewGuid().ToString(),
                    BlackCount = 1,
                    UpdateTime = DateTime.Now,
                    QQNum = QQNum,
                    NickName = mi == null ? string.Empty : mi.Nickname
                });

                db.SaveChanges();
            }
        }

        private static void InitWordList()
        {
            using (var db = new AIDatabase())
            {
                WordList = db.DirtyWord.Select(d => d.Content).ToList();
            }
        }

        public static bool IsInBlackList(long fromQQ)
        {
            if (WordList == null)
            {
                InitWordList();
            }

            using (var db = new AIDatabase())
            {
                var query = db.BlackList.Where(b => b.QQNum == fromQQ);
                if (query.IsNullOrEmpty())
                {
                    return false;
                }

                return query.First().BlackCount >= MaxTolerateCount;
            }
        }

        private static bool IsDirtyWord(string msg)
        {
            foreach (var w in WordList)
            {
                if (msg.Contains(w))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
