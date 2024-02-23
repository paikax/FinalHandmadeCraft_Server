using System.ComponentModel.DataAnnotations;

namespace Data.Entities.Category
{
    public class Category : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
    }
}