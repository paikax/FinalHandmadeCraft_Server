using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Entities.Material
{
    public class Material
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        
        // Change the type of Images to List<string>
        public List<string> Images { get; set; }
        public string CategoryOfMaterial { get; set; }
    }
}