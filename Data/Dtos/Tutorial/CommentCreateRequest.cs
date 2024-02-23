using System;

namespace Data.Dtos.Tutorial
{
    public class CommentCreateRequest
    {
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }
        public string UserId { get; set; }
    }
}