using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AILib.Entities;

namespace AILib
{
    public class DirtyFilter
    {
        private List<string> WordList;

        private int MaxTolerateCount = 10;

        public DirtyFilter()
        {
            InitWordList();
        }

        public bool Filter(long QQNum, string msg)
        {
            // TODO
            return true;
        }

        private void InitWordList()
        {
            // TODO
        }

        public bool IsInBlackList(long fromQQ)
        {
            var query = DbMgr.Query<BlackListEntity>(b => b.QQNum == fromQQ);
            if (query.IsNullOrEmpty())
            {
                return false;
            }

            return query.Count() >= MaxTolerateCount;
        }
    }
}