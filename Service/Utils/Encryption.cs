using System;
using Common.Utils;
using Data.Entities.User;

namespace Service.Utils
{
    public abstract class Encryption
    {
        public static void EncryptPassword<T>(T user, string password) where T : User
        {
            if (string.IsNullOrWhiteSpace(password))
                return;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }


        public static string DecryptPassword<T>(T user, string password) where T : User
        {
            if (string.IsNullOrWhiteSpace(password))
                return string.Empty;

            return SecurityHelper.GetHash(string.Concat(password, user.Salt));
        }
        
        
    }
    
    
}