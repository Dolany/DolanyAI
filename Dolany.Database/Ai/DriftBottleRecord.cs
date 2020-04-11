using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Database.Ai
{
    public class DriftBottleRecord : DbBaseEntity
    {
        public long FromGroup { get; set; }

        public long FromQQ { get; set; }

        public long? ReceivedGroup { get; set; }

        public long? ReceivedQQ { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime SendTime { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? ReceivedTime { get; set; }

        public string Content { get; set; }

        public string SignName { get; set; } = "陌生人";
    }
}
