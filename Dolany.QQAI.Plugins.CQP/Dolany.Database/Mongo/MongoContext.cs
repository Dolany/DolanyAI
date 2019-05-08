using Dolany.Ai.Common;
using MongoDB.Driver;

namespace Dolany.Database
{
    public class MongoContext
    {
        public static MongoContext Instance { get; } = new MongoContext();

        public readonly string MongoConnStr;

        public readonly string MongoDbName;

        private readonly IMongoDatabase _Database;

        private MongoContext()
        {
            MongoConnStr = Configger.Instance["MongoConnStr"];
            MongoDbName = Configger.Instance["MongoDbName"];

            var client = new MongoClient(MongoConnStr);
            _Database = client.GetDatabase(MongoDbName);
        }

        public IMongoCollection<T> Collection<T>()
        {
            return _Database.GetCollection<T>(typeof(T).Name);
        }
    }
}
