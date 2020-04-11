using System;
using Dolany.Database;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.WorldLine.Standard.Ai.Game.Gift
{
    [BsonIgnoreExtraElements]
    public class GlamourRecord : DbBaseEntity
    {
        public long GroupNum { get; set; }

        public long QQNum { get; set; }

        public int Glamour { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? ExpiryTime { get; set; }

        public static GlamourRecord Get(long GroupNum, long QQNum)
        {
            var record = MongoService<GlamourRecord>.GetOnly(p => p.GroupNum == GroupNum && p.QQNum == QQNum);
            if (record != null)
            {
                return record;
            }

            record = new GlamourRecord(){GroupNum = GroupNum, QQNum = QQNum};
            MongoService<GlamourRecord>.Insert(record);

            return record;
        }

        public void Update()
        {
            MongoService<GlamourRecord>.Update(this);
        }
    }
}
