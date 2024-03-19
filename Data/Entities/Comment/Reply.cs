using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Entities.Comment
{
    public class Reply
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Content { get; set; }

        public DateTime TimeStamp { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string CommentId { get; set; }

        // [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
        
        // Reference to the User who wrote the reply
        [BsonIgnore]
        public User.User User { get; set; }
    }
}