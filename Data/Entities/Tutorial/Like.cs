using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Entities.Tutorial
{
    public class Like
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public DateTime TimeStamp { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string TutorialId { get; set; }

        // [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
        
        [BsonIgnore]
        public User.User User { get; set; }
        
    }
}