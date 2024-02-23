using System.ComponentModel.DataAnnotations;

namespace Data.Dtos.Category
{
    public class CreateCategoryDTO
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}