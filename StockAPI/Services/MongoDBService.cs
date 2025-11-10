using MongoDB.Driver;

namespace StockAPI.Services
{
    public class MongoDBService
    {
        readonly IMongoDatabase _database;
        public MongoDBService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
            _database = client.GetDatabase("StockAPIDB");
        }

        // Generic method to get a collection by type name in lowercase
        public IMongoCollection<T> GetCollection<T>()
        {
            return _database.GetCollection<T>(typeof(T).Name.ToLowerInvariant());
        }

    }
}
