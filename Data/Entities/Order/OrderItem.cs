﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Entities.Order
{
    public class OrderItem
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string TutorialId { get; set; }
        public string ProductName { get; set; }
        public string TutorialImageUrl { get; set; }
        public decimal Price { get; set; }
        
        public int Quantity { get; set; }
        public string CreatorId { get; set; }
    }
}