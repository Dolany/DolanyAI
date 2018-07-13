using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AILib.Entities;
using Flexlive.CQP.Framework;

namespace AILib
{
    public class DirtyFilter
    {
        private static List<string> WordList;

        private int MaxTolerateCount = 10;

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
            var query = DbMgr.Query<BlackListEntity>(b => b.QQNum == QQNum);
            if (!query.IsNullOrEmpty())
            {
                var black = query.First();
                black.BlackCount++;
                black.UpdateTime = DateTime.Now;
                DbMgr.Update(black);
                return;
            }

            DbMgr.Insert(new BlackListEntity
            {
                Id = Guid.NewGuid().ToString(),
                BlackCount = 1,
                UpdateTime = DateTime.Now,
                QQNum = QQNum,
                NickName = CQ.GetGroupMemberInfo(GroupNum, QQNum, true).QQName,
                Content = string.Empty
            });
        }

        public static void InitWordList()
        {
            var query = DbMgr.Query<DirtyWordEntity>();
            if (!query.IsNullOrEmpty())
            {
                WordList = query.Select(d => d.Content).ToList();
            }
        }

        public bool IsInBlackList(long fromQQ)
        {
            var query = DbMgr.Query<BlackListEntity>(b => b.QQNum == fromQQ);
            if (query.IsNullOrEmpty())
            {
                return false;
            }

            return query.First().BlackCount >= MaxTolerateCount;
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