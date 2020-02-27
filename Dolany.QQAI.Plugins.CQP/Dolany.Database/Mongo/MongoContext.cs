using Dolany.Ai.Common;
using MongoDB.Driver;

namespace Dolany.Database
{
    public class MongoContext : IDependency
    {
        private readonly IMongoDatabase _Database;

        public MongoContext()
        {
            var mongoConnStr = Configger<AIConfigBase>.Instance.AIConfig.MongoConnStr;
            var mongoDbName = Configger<AIConfigBase>.Instance.AIConfig.MongoDbName;

            var client = new MongoClient(mongoConnStr);
            _Database = client.GetDatabase(mongoDbName);
        }

        public IMongoCollection<T> Collection<T>()
        {
            return _Database.GetCollection<T>(typeof(T).Name);
        }
    }
}
