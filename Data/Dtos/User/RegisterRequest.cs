using System;
using System.ComponentModel.DataAnnotations;
using Common.Constants;
using Newtonsoft.Json;

namespace Data.ViewModels.User
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address"), 
         RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", 
             ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, 
             ErrorMessage = "Password must be at least 6 characters"), RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", 
             ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, and one digit")]
        public string Password { get; set; }
    
        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [RegularExpression(@"^[a-zA-Z]+(([',. -][a-zA-Z ])?[a-zA-Z]*)*$", ErrorMessage = "Invalid first name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [RegularExpression(@"^[a-zA-Z]+(([',. -][a-zA-Z ])?[a-zA-Z]*)*$", ErrorMessage = "Invalid last name")]
        public string LastName { get; set; }
    
        [RegularExpression(@"^\+(?:[0-9] ?){6,14}[0-9]$", ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }
    
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
    
        public string ProfilePhoto { get; set; }

        [StringLength(500, ErrorMessage = "Bio should not exceed 500 characters")]
        [JsonIgnore]
        public string Bio { get; set; }
    
        [StringLength(100, ErrorMessage = "Address should not exceed 100 characters")]
        public string Address { get; set; }
    
        public StringEnums.Roles Role { get; set; }
        
        public string VerificationToken { get; set; }
    }
}