﻿using System;

namespace Data.ViewModels.User
{
    public class UpdateRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string ProfilePhoto { get; set; }
        public string Bio { get; set; }
    }
}