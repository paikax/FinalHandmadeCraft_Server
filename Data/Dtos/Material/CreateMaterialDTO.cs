using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Dtos.Material
{
    public class CreateMaterialDTO
    {
        [Required]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        
        // Change the type of Images to List<string>
        public List<string> Images { get; set; }
        public string CategoryOfMaterial { get; set; }
    }
}