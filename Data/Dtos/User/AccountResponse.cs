using System;

namespace Data.ViewModels.User
{
    public class AccountResponse
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProfilePhoto { get; set; }
        
        public string Bio { get; set; }
        
        public string PhoneNumber { get; set; } 
        
        public DateTime DateOfBirth { get; set; } 
        
        public string Address { get; set; } 
        
        public bool IsVerified { get; set; }
        
        public bool IsDeleted { get; set; }
        
        public bool IsPremium { get; set; } = false;
    }
}