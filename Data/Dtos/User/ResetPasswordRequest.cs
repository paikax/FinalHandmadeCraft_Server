using System.ComponentModel.DataAnnotations;

namespace Data.ViewModels.User
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, 
             ErrorMessage = "Password must be at least 6 characters"), RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", 
             ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, and one digit")]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}