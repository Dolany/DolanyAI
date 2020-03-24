using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Database.Sqlite.Model
{
    public class HelloCache
    {
        public long GroupNum { get; set; }
        public long QQNum { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastUpdateTime { get; set; }
    }
}
