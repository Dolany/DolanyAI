using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Database.Ai
{
    [BsonIgnoreExtraElements]
    public class AlermClock : DbBaseEntity
    {
        public long GroupNumber { get; set; }
        public long Creator { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; }
        public int AimHourt { get; set; }
        public int AimMinute { get; set; }
        public string Content { get; set; }
        public string BindAi { get; set; }
    }
}
