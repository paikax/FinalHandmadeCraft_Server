using System;
using System.Security.Cryptography;
using System.Text;
namespace Common.Utils
{
    public static class SecurityHelper
    {
        public static string GetHash(string text)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        public static string GetSalt()
        {
            byte[] bytes = new byte[128 / 8];
            using (var keyGenerator = RandomNumberGenerator.Create())
            {
                keyGenerator.GetBytes(bytes);
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        public static string GetCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            using (var random = new RNGCryptoServiceProvider())
            {
                var data = new byte[length];
                random.GetBytes(data);
                var result = new StringBuilder(length);
                foreach (byte b in data)
                {
                    result.Append(chars[b % chars.Length]);
                }
                return result.ToString();
            }
        }
    }
}