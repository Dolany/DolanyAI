using MongoDB.Driver;

namespace Dolany.Database
{
    public class MongoContext
    {
        public static MongoContext Instance { get; } = new MongoContext();

        private readonly IMongoDatabase _Database;

        private MongoContext()
        {
            var client = new MongoClient(
                //"mongodb://dolany:2160727@dolanycluster-shard-00-00-qm6a0.azure.mongodb.net:27017,dolanycluster-shard-00-01-qm6a0.azure.mongodb.net:27017,dolanycluster-shard-00-02-qm6a0.azure.mongodb.net:27017/test?ssl=true&replicaSet=DolanyCluster-shard-0&authSource=admin&retryWrites=true");
                "mongodb://localhost");
            _Database = client.GetDatabase("DolanyAI");
        }

        public IMongoCollection<T> Collection<T>()
        {
            return _Database.GetCollection<T>(typeof(T).Name);
        }
    }
}
