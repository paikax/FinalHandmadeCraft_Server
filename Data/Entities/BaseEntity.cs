using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class BaseEntity
    {
        protected BaseEntity()
        {
            Id = Guid.NewGuid().ToString();
            CreateAt = DateTime.UtcNow;
        }
        [Key] 
        public string Id { get; set; }
        public DateTime CreateAt { get; set; }

        public DateTime? UpdateAt { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeleteAt { get; set; }
    }
}