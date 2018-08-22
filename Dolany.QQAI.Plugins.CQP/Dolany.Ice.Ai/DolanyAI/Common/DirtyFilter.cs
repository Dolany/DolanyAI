using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dolany.Ice.Ai.DolanyAI.Db;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class DirtyFilter
    {
        private static List<string> WordList;

        private readonly int MaxTolerateCount = 10;

        public DirtyFilter()
        {
            if (WordList == null)
            {
                WordList = new List<string>();
            }
            InitWordList();
        }

        public bool Filter(long GroupNum, long QQNum, string msg)
        {
            if (IsDirtyWord(msg))
            {
                AddInBlackList(GroupNum, QQNum);
                return false;
            }
            return true;
        }

        private void AddInBlackList(long GroupNum, long QQNum)
        {
            using (AIDatabase db = new AIDatabase())
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

                db.BlackList.Add(new BlackList
                {
                    Id = Guid.NewGuid().ToString(),
                    BlackCount = 1,
                    UpdateTime = DateTime.Now,
                    QQNum = QQNum,
                    NickName = Utility.GetMemberInfo(new GroupMsgDTO
                    {
                        FromGroup = GroupNum,
                        FromQQ = QQNum
                    }).Nickname
                });

                db.SaveChanges();
            }
        }

        public static void InitWordList()
        {
            using (AIDatabase db = new AIDatabase())
            {
                WordList = db.DirtyWord.Select(d => d.Content).ToList();
            }
        }

        public bool IsInBlackList(long fromQQ)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.BlackList.Where(b => b.QQNum == fromQQ);
                if (query.IsNullOrEmpty())
                {
                    return false;
                }

                return query.First().BlackCount >= MaxTolerateCount;
            }
        }

        private bool IsDirtyWord(string msg)
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