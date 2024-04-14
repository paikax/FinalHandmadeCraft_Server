using System;
using System.Collections.Generic;
using Data.ViewModels.User;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Entities.Tutorial
{
    public class Tutorial
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Title { get; set; }
        public string DifficultLevel { get; set; }
        public string CompletionTime { get; set; }
        public string Instruction { get; set; }
        public string VideoUrl { get; set; }
        public string CategoryId { get; set; }
        
        public string Material { get; set; }
        public string CreatedById { get; set; }

        public List<Material.Material> Materials { get; set; }
        public decimal Price { get; set; }
        public DateTime UploadTime { get; set; }    
        
        public List<Comment.Comment> Comments { get; set; } = new List<Comment.Comment>();
        public List<Like> Likes { get; set; } = new List<Like>();
        
    }
}