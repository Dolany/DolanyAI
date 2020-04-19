using System;
using Dolany.Database;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.WorldLine.Standard.Ai.Game.Archaeology
{
    public class Archaeologist : DbBaseEntity
    {
        public long QQNum { get; set; }

        public int SAN { get; set; } = 100;

        public int CurSAN { get; set; } = 100;

        public int Ice { get; set; } = 1;

        public int Flame { get; set; } = 1;

        public int Lightning { get; set; } = 1;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? RebornTime { get; set; } = DateTime.Now;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime SANRefreshTime { get; set; } = DateTime.Now;

        public bool IsDead => RebornTime.HasValue && RebornTime > DateTime.Now;

        public static Archaeologist Get(long QQNum)
        {
            var rec = MongoService<Archaeologist>.GetOnly(p => p.QQNum == QQNum);
            if (rec != null)
            {
                if (rec.RefreshSAN())
                {
                    rec.Update();
                }

                return rec;
            }

            rec = new Archaeologist(){QQNum = QQNum};
            MongoService<Archaeologist>.Insert(rec);
            return rec;
        }

        public bool RefreshSAN()
        {
            if (SANRefreshTime.Date == DateTime.Today)
            {
                return false;
            }

            CurSAN = SAN;
            SANRefreshTime = DateTime.Now;
            return true;
        }

        public void Update()
        {
            MongoService<Archaeologist>.Update(this);
        }
    }
}
