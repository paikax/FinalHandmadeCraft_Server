using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Entities.ForumPost
{
    public class ForumPost
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime TimeStamp { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserPostId { get; set; }
    }
}