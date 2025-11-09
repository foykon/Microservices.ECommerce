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
    }
}
