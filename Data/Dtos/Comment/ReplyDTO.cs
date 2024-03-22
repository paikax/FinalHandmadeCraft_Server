using System;
using Data.Entities.User;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Dtos.Comment
{
    public class ReplyDTO
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }
        public string UserId { get; set; }
        public string CommentId { get; set; }
        [BsonIgnore]
        public User User { get; set; }
        public string UserName { get; set; }
        public string UserProfilePhoto { get; set; }
    }
}