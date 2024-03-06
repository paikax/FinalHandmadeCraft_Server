using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Entities.Notification
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public string BuyerId { get; set; }
        public string SellerId { get; set; }
        
        public string Title { get; set; }
        
        public string Message { get; set; }
        
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAt { get; set; }
    }
}