using System;

namespace Data.Dtos.Comment
{
    public class ReplyCreateRequest
    {
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }
        public string UserId { get; set; }
    }
}