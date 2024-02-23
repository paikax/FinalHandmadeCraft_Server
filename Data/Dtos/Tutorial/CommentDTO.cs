﻿using System;
using Data.Entities.User;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Dtos.Tutorial
{
    public class CommentDTO
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
        
        // Reference to the User who wrote the comment
        [BsonIgnore]
        public User User { get; set; }
    }
}