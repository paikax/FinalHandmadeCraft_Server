using MongoDB.Driver;

namespace Data.Context
{
    public class MongoRepository
    {
        private readonly IMongoClient _mongoClient;

        public MongoRepository(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
        }

        public IMongoDatabase GetDatabase(string databaseName)
        {
            return _mongoClient.GetDatabase(databaseName);
        }
    }
}