using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Entities.Order
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public string UserId { get; set; }
        
        // Include the list of OrderItems directly in the Order class
        public List<OrderItem> Items { get; set; }
        
        public int Quantity { get; set; }
        
        public decimal TotalPrice { get; set; }
        
        public string Address { get; set; }
    }
}