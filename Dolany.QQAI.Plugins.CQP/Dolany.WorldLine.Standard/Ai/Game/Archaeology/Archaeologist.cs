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

        public int Ice { get; set; } = 10;

        public int Flame { get; set; } = 10;

        public int Lightning { get; set; } = 10;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? RebornTime { get; set; } = DateTime.Now;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime SANRefreshTime { get; set; } = DateTime.Now;

        public bool IsDead => RebornTime > DateTime.Now;

        public static Archaeologist Get(long QQNum)
        {
            var rec = MongoService<Archaeologist>.GetOnly(p => p.QQNum == QQNum);
            if (rec != null)
            {
                if (rec.SANRefreshTime.Date == DateTime.Today)
                {
                    return rec;
                }

                rec.CurSAN = rec.SAN;
                rec.SANRefreshTime = DateTime.Now;
                rec.Update();
                return rec;
            }

            rec = new Archaeologist(){QQNum = QQNum};
            MongoService<Archaeologist>.Insert(rec);
            return rec;
        }

        public void Update()
        {
            MongoService<Archaeologist>.Update(this);
        }
    }
}
