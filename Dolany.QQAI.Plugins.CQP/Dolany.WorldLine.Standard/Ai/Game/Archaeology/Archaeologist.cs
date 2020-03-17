using System;
using Dolany.Database;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.WorldLine.Standard.Ai.Game.Archaeology
{
    public class Archaeologist : DbBaseEntity
    {
        public long QQNum { get; set; }

        public int Level { get; set; }

        public int Exp { get; set; }

        public int Power { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? RecoverTime { get; set; }

        public static Archaeologist Get(long QQNum)
        {
            var rec = MongoService<Archaeologist>.GetOnly(p => p.QQNum == QQNum);
            if (rec != null)
            {
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
