using System.ComponentModel.DataAnnotations;

namespace Data.ViewModels.User
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}