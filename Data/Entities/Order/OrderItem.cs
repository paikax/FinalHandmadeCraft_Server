using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Entities.Order
{
    public class OrderItem
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string TutorialId { get; set; }
        public decimal Price { get; set; }
        
        public int Quantity { get; set; }
        
    }
}