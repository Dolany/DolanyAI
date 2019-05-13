using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Database.Ai
{
    [BsonIgnoreExtraElements]
    public class HelloRecord : DbBaseEntity
    {
        public long GroupNum { get; set; }
        public long QQNum { get; set; }
        public string Content { get; set; }
    }
}
