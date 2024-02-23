using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities.User
{
    public class UserFollower
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        public string UserId { get; set; }
        public virtual User User { get; set; }
        
        public string FollowerId { get; set; }
        public virtual User Follower { get; set; }
    }
}