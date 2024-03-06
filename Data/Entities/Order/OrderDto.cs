using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Entities.Order
{
    public class OrderDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public List<OrderItemDto> Items { get; set; }
        // public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string Address { get; set; }
        public string BuyerEmail { get; set; } 
        
        public string SellerEmail { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime OrderDate { get; set; } 
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime LastUpdated { get; set; } 
        public string CreatorId { get; set; }
    }
}