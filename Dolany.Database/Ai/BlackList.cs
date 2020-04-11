using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Database.Ai
{
    public class BlackList : DbBaseEntity
    {
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdateTime { get; set; }
        public long QQNum { get; set; }
        public int BlackCount { get; set; }
        public string NickName { get; set; }
    }
}
