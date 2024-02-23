using System.ComponentModel.DataAnnotations;

namespace Data.Entities.User
{
    public class PayPalSetupRequest
    {
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string ClientSecret { get; set; }
    }
}