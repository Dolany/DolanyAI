using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Ai.Doremi.Common
{
    public class DirtyFilter : IDataMgr
    {
        private string[] WordList;

        private List<long> BlackList;

        public static DirtyFilter Instance { get; } = new DirtyFilter();

        private const int MaxTolerateCount = 10;

        private DirtyFilter()
        {
            RefreshData();
            //DataRefresher.Instance.Register(this);
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
            AddInBlackList(MsgDTO.FromQQ);
            return false;
        }

        private void AddInBlackList(long QQNum)
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

        public void RefreshData()
        {
            WordList = CommonUtil.ReadJsonData<Dictionary<string, string[]>>("DirtyWordData").First().Value;
            BlackList = MongoService<BlackList>.Get(p => p.BlackCount >= MaxTolerateCount).Select(p => p.QQNum).ToList();
        }
    }
}
