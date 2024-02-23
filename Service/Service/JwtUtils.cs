using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Common.Constants;
using Data.Entities.User;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.IServices;

namespace Service.Service
{
    public class JwtUtils : IJwtUtils
{
    private readonly IConfiguration _configuration;

    public JwtUtils(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    // public string GenerateToken(User user)
    // {
    //     var tokenHandler = new JwtSecurityTokenHandler();
    //     var key = Encoding.ASCII.GetBytes(AppSettings.Secret);
    //     var tokenDescriptor = new SecurityTokenDescriptor()
    //     {
    //         Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
    //         Expires = DateTime.UtcNow.AddDays(7),
    //         SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
    //             SecurityAlgorithms.HmacSha256Signature)
    //     };
    //     var tokens = tokenHandler.CreateToken(tokenDescriptor);
    //     return tokenHandler.WriteToken(tokens);
    // }
    
    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Secret").Value);
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new Claim[] 
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
                // You can add more claims if required
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public int? ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var keyString = AppSettings.Secret;

        // Ensure the secret key is not null or empty
        if (string.IsNullOrWhiteSpace(keyString))
            throw new InvalidOperationException("JWT Secret is not set.");

        var key = Encoding.ASCII.GetBytes(keyString);
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            // Ensure the token has the claim we are looking for
            var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
            if (userIdClaim == null)
            {
                throw new SecurityTokenException("Invalid token: User ID claim not found.");
            }

            var userId = int.Parse(userIdClaim.Value);

            return userId;
        }
        catch (Exception ex)
        {
            // Log the exception details here
            // Example: _logger.LogError(ex, "Error validating JWT token");
            return null;
        }
    }


}
}