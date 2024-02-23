using System;
using System.Collections.Generic;
using Data.Entities.User;

namespace Data.ViewModels.User
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicture { get; set; }
        public string PhoneNumber { get; set; } 
        public DateTime DateOfBirth { get; set; } 
        public string Address { get; set; }
        public bool IsPremium { get; set; }
    }
}