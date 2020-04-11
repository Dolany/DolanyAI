using System;
using System.Linq;
using Dolany.Database;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.WorldLine.Standard.Ai.Game.Gift
{
    public class IntimateRelationshipRecord : DbBaseEntity
    {
        public long GroupNum { get; set; }

        public long[] QQPair { get; set; }

        public int Value { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? ExpiryTime { get; set; }

        public string Name { get; set; }

        public static int GetSumIntimate(long GroupNum, long firstQQ, long secondQQ)
        {
            var records = MongoService<IntimateRelationshipRecord>.Get(p => p.GroupNum == GroupNum && p.QQPair.Contains(firstQQ) && p.QQPair.Contains(secondQQ));
            return records.Sum(p => p.Value);
        }

        public void Insert()
        {
            MongoService<IntimateRelationshipRecord>.Insert(this);
        }
    }
}
