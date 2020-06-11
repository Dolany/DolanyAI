using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Database.Ai
{
    [BsonIgnoreExtraElements]
    public class HelloRecord : DbBaseEntity
    {
        public long GroupNum { get; set; }
        public long QQNum { get; set; }
        public string Content { get; set; }

        public static HelloRecord Get(long GroupNum, long QQNum)
        {
            return MongoService<HelloRecord>.GetOnly(p => p.GroupNum == GroupNum && p.QQNum == QQNum);
        }

        public void Update()
        {
            MongoService<HelloRecord>.Update(this);
        }

        public void Insert()
        {
            MongoService<HelloRecord>.Insert(this);
        }

        public void Remove()
        {
            MongoService<HelloRecord>.Delete(this);
        }
    }
}
