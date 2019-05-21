using System;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Game.Gift
{
    public class GlamourRecord : DbBaseEntity
    {
        public long GroupNum { get; set; }

        public long QQNum { get; set; }

        public int Glamour { get; set; }

        public DateTime ExpiryTime { get; set; }

        public static GlamourRecord Get(long GroupNum, long QQNum)
        {
            var record = MongoService<GlamourRecord>.GetOnly(p => p.GroupNum == GroupNum && p.QQNum == QQNum);
            if (record != null)
            {
                return record;
            }

            record = new GlamourRecord(){GroupNum = GroupNum, QQNum = QQNum, ExpiryTime = DateTime.Today.AddDays(1 - DateTime.Now.Day).AddMonths(1)};
            MongoService<GlamourRecord>.Insert(record);

            return record;
        }

        public void Update()
        {
            MongoService<GlamourRecord>.Update(this);
        }
    }
}
