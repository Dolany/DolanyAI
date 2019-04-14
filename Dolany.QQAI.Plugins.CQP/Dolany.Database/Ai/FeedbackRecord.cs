using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Database.Ai
{
    [BsonIgnoreExtraElements]
    public class FeedbackRecord : DbBaseEntity
    {
        public long GroupNum { get; set; }

        public long QQNum { get; set; }

        public DateTime UpdateTime { get; set; }

        public string Content { get; set; }
    }
}
