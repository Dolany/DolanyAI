using MongoDB.Driver;

namespace Dolany.Database
{
    public class MongoContext
    {
        public static MongoContext Instance { get; } = new MongoContext();

        private readonly IMongoDatabase _Database;

        private MongoContext()
        {
            var client = new MongoClient("mongodb://localhost");
            _Database = client.GetDatabase("DolanyAI");
        }

        public IMongoCollection<T> Collection<T>()
        {
            return _Database.GetCollection<T>(typeof(T).Name);
        }
    }
}
