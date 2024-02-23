using System.Text.Json.Serialization;
using Common.Constants;

namespace Data.ViewModels.User
{
    public class AuthenticationResponse
    {
        public string Id { get; set; }
        public string JwtToken { get; set; }
        
        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }
        

        public AuthenticationResponse()
        {
            // Parameterless constructor
        }
        
        public AuthenticationResponse(Entities.User.User user, string jwtToken, string refreshToken)
        {
            Id = user.Id;
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
            Role = user.Role;
            IsVerified = user.IsVerified;
            EmailConfirmed = user.EmailConfirmed;
            ProfilePhoto = user.ProfilePhoto;
            IsPremium = user.IsPremium;
        }
        
        public string Email { get; set; }
        public string FirstName { get; set; }
        public  string LastName { get; set; }
        public bool EmailConfirmed { get; set; }
        public StringEnums.Roles Role { get; set; }
        public bool IsVerified { get; set; }
        public string ProfilePhoto { get; set; }
        
        public bool IsPremium { get; set; } = false;
    }
}