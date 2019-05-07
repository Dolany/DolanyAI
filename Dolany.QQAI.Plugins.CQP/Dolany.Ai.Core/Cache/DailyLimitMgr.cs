using System.Collections.Generic;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Cache
{
    public class DailyLimitMgr
    {
        private readonly object Lock = new object();

        private readonly Dictionary<long, DailyLimitRecord> RecordDic = new Dictionary<long, DailyLimitRecord>();

        public static DailyLimitMgr Instance { get; } = new DailyLimitMgr();

        private DailyLimitMgr()
        {

        }

        public DailyLimitRecord this[long QQNum]
        {
            get
            {
                lock (Lock)
                {
                    if (RecordDic.ContainsKey(QQNum))
                    {
                        return RecordDic[QQNum];
                    }

                    var record = DailyLimitRecord.Get(QQNum);
                    RecordDic.Add(QQNum, record);
                    return record;
                }
            }
        }
    }
}
