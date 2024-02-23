using System.ComponentModel.DataAnnotations;

namespace Data.ViewModels.User
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}