using System;
using Data.Entities.User;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Dtos.Tutorial
{
    public class LikeDTO
    {
        public DateTime TimeStamp { get; set; }
        public string UserId { get; set; }
        [BsonIgnore]
        public User User { get; set; }
    }
}