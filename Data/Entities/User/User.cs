using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Constants;
using Newtonsoft.Json;

namespace Data.Entities.User
{
    public class User : BaseEntity
    {
        // validate fields
        public StringEnums.Roles Role { get; set; }
        [JsonIgnore] public string PasswordHash { get; set; }
        
        [JsonIgnore] public string Salt { get; set; }
        
        [JsonIgnore] public List<RefreshToken> RefreshTokens { get; set; }
        public string VerificationToken { get; set; }   
        public bool EmailConfirmed { get; set; }
        public DateTime? VerifiedAt { get; set; }
        
        public bool IsVerified => VerifiedAt.HasValue || PasswordReset.HasValue;
        public string ResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public DateTime? PasswordReset { get; set; }

        public bool OwnsToken(string token)
        {
            return this.RefreshTokens?.Find(x => x.Token == token) != null;
        }
        
        // ----------------- main fields
        [EmailAddress] public string Email { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProfilePhoto { get; set; }
        
        public string Bio { get; set; }
        
        public string PhoneNumber { get; set; } 
        
        public DateTime DateOfBirth { get; set; } 
        
        public string Address { get; set; } 
        
        public bool IsPremium { get; set; } = false;
        
        public bool IsPayPalLinked { get; set; } = false;
        public string PayPalClientId { get; set; }
        public string PayPalClientSecret { get; set; }
        
        public string PayPalEmail { get; set; }
        
        public virtual ICollection<UserFollower> Followers { get; set; } = new List<UserFollower>();
        public virtual ICollection<UserFollower> Following { get; set; } = new List<UserFollower>();
    }
}