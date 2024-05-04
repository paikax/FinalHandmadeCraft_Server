using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Dtos.Tutorial
{
    public class TutorialCreateRequest
    {
        public string Title { get; set; }
        public string DifficultLevel { get; set; }
        public string CompletionTime { get; set; }
        public string Instruction { get; set; }
        public string VideoUrl { get; set; }
        public string CategoryId { get; set; }
        public string Material { get; set; }
        public List<string> MaterialIds { get; set; }
        public string CreatedById { get; set; }
        public decimal Price { get; set; }
        public DateTime UploadTime { get; set; }
        
        public List<string> Comments { get; set; } = new List<string>();
        public int Rating { get; set; } = 0;
        public int Quantity { get; set; }
    }
}