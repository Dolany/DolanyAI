using System.Collections.Generic;
using System.Linq;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Game.SegmentAttach
{
    public class SegmentRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public string Segment { get; set; }

        public Dictionary<string, int> TreasureRecord { get; set; } = new Dictionary<string, int>();

        public int FinalTreasureCount { get; set; }

        public bool CanOpenFinalTreasure
        {
            get
            {
                if (TreasureRecord.Count < SegmentMgr.Instance.Treasures.Count)
                {
                    return false;
                }

                var totalCanOpenCount = TreasureRecord.Values.Min();
                return totalCanOpenCount > FinalTreasureCount;
            }
        }

        public static SegmentRecord Get(long QQNum)
        {
            var record = MongoService<SegmentRecord>.GetOnly(p => p.QQNum == QQNum);
            if (record != null)
            {
                return record;
            }

            record = new SegmentRecord(){QQNum = QQNum};
            MongoService<SegmentRecord>.Insert(record);
            return record;
        }

        public void AddTreasureRecord(string treasure)
        {
            if (!TreasureRecord.ContainsKey(treasure))
            {
                TreasureRecord.Add(treasure, 0);
            }

            TreasureRecord[treasure]++;
        }

        public void Update()
        {
            MongoService<SegmentRecord>.Update(this);
        }
    }
}
