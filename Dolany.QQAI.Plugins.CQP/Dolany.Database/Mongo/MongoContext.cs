using Dolany.Ai.Common;
using MongoDB.Driver;

namespace Dolany.Database
{
    public class MongoContext
    {
        public static MongoContext Instance { get; } = new MongoContext();

        private readonly IMongoDatabase _Database;

        private MongoContext()
        {
            var mongoConnStr = Configger.Instance.AIConfig.MongoConnStr;
            var mongoDbName = Configger.Instance.AIConfig.MongoDbName;

            var client = new MongoClient(mongoConnStr);
            _Database = client.GetDatabase(mongoDbName);
        }

        public IMongoCollection<T> Collection<T>()
        {
            return _Database.GetCollection<T>(typeof(T).Name);
        }
    }
}
