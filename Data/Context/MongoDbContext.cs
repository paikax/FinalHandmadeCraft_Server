using Data.Entities.Category;
using Data.Entities.Comment;
using Data.Entities.ForumPost;
using Data.Entities.Material;
using Data.Entities.Order;
using Data.Entities.Tutorial;
using MongoDB.Driver;

namespace Data.Context

{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IMongoClient mongoClient, string databaseName)
        {
            _database = mongoClient.GetDatabase(databaseName);
        }

        public IMongoCollection<Category> Categories => _database.GetCollection<Category>("categories");
        public IMongoCollection<Tutorial> Tutorials => _database.GetCollection<Tutorial>("tutorials");
        public IMongoCollection<Material> Materials => _database.GetCollection<Material>("materials");
        public IMongoCollection<Order> Orders => _database.GetCollection<Order>("orders");
        public IMongoCollection<OrderItem> OrderItems => _database.GetCollection<OrderItem>("orderitems");
        public IMongoCollection<ForumPost> ForumPosts => _database.GetCollection<ForumPost>("forumposts");
        // public IMongoCollection<Comment> Comments => _database.GetCollection<Comment>("comments");
        // Add other collections as needed

        // Add any additional methods or configurations specific to MongoDB
    }
}