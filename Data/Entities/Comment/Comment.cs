using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Entities.Comment
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Content { get; set; }

        public DateTime TimeStamp { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string PostId { get; set; }

        // [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
        
        // Reference to the User who wrote the comment
        [BsonIgnore]
        public User.User User { get; set; }
        
    }
}