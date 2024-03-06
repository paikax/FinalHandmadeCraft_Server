using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Entities.Order
{
    public class OrderRequest
    {
        public string UserId { get; set; }
        public List<OrderItem> Items { get; set; }
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