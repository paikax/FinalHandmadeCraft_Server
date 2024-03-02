using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Data.Context;
using Data.Entities.User;
using Data.ViewModels.User;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Common.Constants;
using Data.Dtos.Tutorial;
using Data.Entities;
using Data.Entities.Tutorial;
using MongoDB.Driver;
using Service.IServices;
using Service.Utils;

namespace Service.Service
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly IJwtUtils _jwtUtils;
        private readonly IMapper _mapper;
        private readonly ISendMailService _sendMailService;
        private readonly IConfiguration _configuration;
        private readonly MongoDbContext _mongoDbContext;

        public UserService(AppDbContext db, IJwtUtils jwtUtils, IMapper mapper,
            ISendMailService sendMailService,
                IConfiguration configuration,
            MongoDbContext mongoDbContext
            )
        {
            _sendMailService = sendMailService; 
            _db = db;
            _jwtUtils = jwtUtils;
            _mapper = mapper;
            _configuration = configuration;
            _mongoDbContext = mongoDbContext;
        }
        
        public async Task<AuthenticationResponse> Authenticate(AuthenticationRequest model, string ipAddress)
        {
            try
            {
                // Check if Email and Password are not null or empty
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                {
                    throw new Exception("Email and Password are required for authentication.");
                }
                
        
                var user = await _db.Users.Include(user => user.RefreshTokens).SingleOrDefaultAsync(x => x.Email == model.Email);
        
                if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    throw new Exception("Username or password is incorrect");
                }
                
                // authentication successful so generate JWT and Refresh tokens
                var jwtToken = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken(ipAddress);
                
                
                user.RefreshTokens ??= new List<RefreshToken>();
                user.RefreshTokens.Add(refreshToken);
                
                RemoveOldRefreshTokens(user);
                
                // Update refresh tokens
                await UpdateRefreshToken(user.Id, user.RefreshTokens, refreshToken.Token, ipAddress);
                
                // var response = _mapper.Map<AuthenticationResponse>(user);
                // response.JwtToken = _jwtUtils.GenerateToken(user);
                // return response;

                return new AuthenticationResponse(user, jwtToken, refreshToken.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Authentication error: {ex.Message}");
                throw;
            }
        }

        public async Task<AuthenticationResponse> RefreshToken(string token, string ipAddress)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => 
                u.RefreshTokens.Any(t => t.Token == token && t.IsActive && !t.IsRevoked));

            if (user == null)
                throw new ApplicationException("Invalid token.");

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            // Revoke current refresh token
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedBy = ipAddress; 

            // Generate new refresh token and save
            var newRefreshToken = GenerateRefreshToken(ipAddress);
            user.RefreshTokens.Add(newRefreshToken);
            await _db.SaveChangesAsync();

            // Generate new jwt
            var jwtToken = GenerateJwtToken(user);

            return new AuthenticationResponse(user, jwtToken, newRefreshToken.Token);
        }

        
        private async Task UpdateRefreshToken(string userId, List<RefreshToken> refreshTokens, string newRefreshToken, string ipAddress)
        {
            // Get the user from the database
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            // Remove old refresh tokens
            RemoveOldRefreshTokens(user);

            // Add the new refresh token
            var refreshToken = new RefreshToken
            {
                Token = newRefreshToken,
                Expires = DateTime.Now.AddDays(10),
                Created = DateTime.Now,
                CreatedByIp = ipAddress 
            };

            user.RefreshTokens.Add(refreshToken);

            // Save changes to the database
            await _db.SaveChangesAsync();
        }

        public async Task<bool> RevokeToken(string token, string ipAddress)
        {
            var user = await _db.Users.Include(user => user.RefreshTokens).FirstOrDefaultAsync(u =>
                u.RefreshTokens.Any(t => t.Token == token) && !u.IsDeleted);
            
            // return false if no user found with token
            if (user == null) return false;

            var refreshToken = user.RefreshTokens.Single(r => r.Token == token);
            
            // return false if token is not active
            if (!refreshToken.IsActive) return false;
            
            // revoke token and save
            refreshToken.Revoked = DateTime.Now;
            refreshToken.RevokedBy = ipAddress;

            await _db.SaveChangesAsync();
            
            return true;
        }


        public async Task Activate(string id)
        {
            // var account = await GetDeleteAccount(id);
            // account.IsDeleted = false;
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _db.Users.ToListAsync();
        }
        
        public async Task<bool> IsFollowing(string followerId, string userId)
        {
            return await _db.UserFollowers.AnyAsync(uf => uf.UserId == userId && uf.FollowerId == followerId);
        }


        public async Task<User> GetById(string id)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        }


        public async Task Register(User model, string password, string origin)
        {
            try
            {
                if (await _db.Users.AnyAsync(x => x.Email == model.Email))
                {
                    throw new Exception($"Email '{model.Email}' is already taken");
                }
                
                // hash password
                Encryption.EncryptPassword(model, password);
                model.VerificationToken = RandomTokenString();

                await _db.Users.AddAsync(model);
                await _db.SaveChangesAsync();
                
                // Use a background service for sending emails
                // var origin = "https://localhost:44346";
                BackgroundJob.Enqueue(() => SendVerificationEmail(model, origin));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var account = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());
            
            // always return ok response to prevent email enumeration
            if (account == null) return;
            
            // create reset token that expire after 12 hours
            account.ResetToken = RandomTokenString();
            account.ResetTokenExpires = DateTime.Now.AddHours(12);

            await _db.SaveChangesAsync();
            
            // send email
            await SendPasswordResetEmail(account, origin);
        }
        
        public async Task ValidateResetToken(ValidateResetTokenRequest model)
        {
            var account = await _db.Users.FirstOrDefaultAsync(u =>
                u.ResetToken == model.Token && 
                u.ResetTokenExpires > DateTime.UtcNow);

            if (account == null)
                throw new ApplicationException("Invalid token.");
        }

        
        public async Task ResetPassword(ResetPasswordRequest model)
        {
            var account = await _db.Users.FirstOrDefaultAsync(u => u.ResetToken == model.Token &&
                                                                   u.ResetTokenExpires > DateTime.Now);
            if (account == null)
                throw new Exception("Invalid token");
            
            // hash password
            Encryption.EncryptPassword(account, model.Password);
            
            // update password and remove reset token
            account.PasswordReset = DateTime.Now;
            account.ResetToken = null;
            account.ResetTokenExpires = null;
            account.UpdateAt = DateTime.Now;

            await _db.SaveChangesAsync();
        }


        public async Task Update(string id, UpdateRequest model)
        {
            try
            {

                var user =  await _db.Users.FirstOrDefaultAsync(u=>u.Id == id);

                // validate
                var userList = await _db.Users.AnyAsync(x => x.Email == model.Email);
                if (userList)
                    throw new Exception("UserName '" + model.Email + "' is already taken");

                // hash password if it was entered
                if (!string.IsNullOrEmpty(model.Password))
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

                // copy model to user and save
                _mapper.Map(model, user);

                await _db.SaveChangesAsync();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task Delete(string id)
        {
            try
            {
                var user =  await _db.Users.FirstOrDefaultAsync(u=>u.Id == id);
                if (user == null) throw new KeyNotFoundException("User not found");
                _db.Users.Remove(user);
                _db.SaveChanges();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        
        
        
        // --------------  HELPER METHOD ----------------------
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Secret").Value);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim("UserId", user.Id),
                    new Claim("Role", user.Role.ToString())
                    // You can add more claims if required
                }),
                Expires = DateTime.Now.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        
        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[64];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.Now.AddDays(10),
                Created = DateTime.Now,
                CreatedByIp = ipAddress
            };
        }
        
        private static void RemoveOldRefreshTokens(User account)
        {
            account.RefreshTokens.RemoveAll(r => 
                !r.IsActive
                && r.Created.AddDays(AppSettings.RefreshTokenTtl) <= DateTime.Now);
        }
        
        
        
        public async Task<User> VerifyEmail(string token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);

            if (user == null)
                throw new Exception("Invalid verification token.");

            if (user.EmailConfirmed)
                throw new Exception("Email is already confirmed.");

            user.EmailConfirmed = true;
            user.VerifiedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            return user;
        }

        public async Task<bool> EmailExists(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email is required.", nameof(email));
            }

            // Check if any user with the given email exists
            var emailExists = await _db.Users.AnyAsync(u => u.Email == email);
            return emailExists;
        }

        public async Task Logout(string refreshToken)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => 
                u.RefreshTokens.Any(t => t.Token == refreshToken));

            if (user == null) 
                return;

            var token = user.RefreshTokens.Single(x => x.Token == refreshToken);

            if (!token.IsActive) 
                return;

            // revoke token and save
            token.Revoked = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }


        public async Task SendVerificationEmail(User user, string origin)
        {
            var verificationUrl = $"{origin}/api/User/verify-email?token={user.VerificationToken}";
            var message = $@"<p>Please click the below link to verify your account.</p>
                             <p>
                                <a 
                                    style=""display: inline-block;
                                            padding: 10px 20px;
                                            background-color: #00cc66;
                                            color: #fff;
                                            text-decoration: none;
                                            font-weight: bold;
                                            border: none;
                                            border-radius: 5px; "" 
                                    href=""{verificationUrl}"">Verify Email
                                </a>
                             </p>";

            try
            {
                await _sendMailService.SendMailAsync(user.Email, "Verify Email", message);
            }
            catch (Exception ex)
            {
                // Log or handle exceptions here
                throw new ApplicationException($"Unable to send verification email: {ex.Message}");
            }
        }
        
        private string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private async Task SendPasswordResetEmail(User account, string origin)
        {
            // string message;
            // if (!string.IsNullOrEmpty(origin))
            // {
            //     var resetUrl = $"{origin}/reset-password?token={account.ResetToken}";
            //     message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
            //                  <p>
            //                     <a 
            //                         style=""display: inline-block;
            //                                 padding: 10px 20px;
            //                                 background-color: #00cc66;
            //                                 color: #fff;
            //                                 text-decoration: none;
            //                                 font-weight: bold;
            //                                 border: none;
            //                                 border-radius: 5px; "" 
            //                         href=""{resetUrl}"">Reset password
            //                     </a>
            //                  </p>";
            // }
            // else
            // {
            //     message = $@"<p>Please use the below token to reset your password with the <code>/reset-password</code> api route:</p>
            //                  <p><code>{account.ResetToken}</code></p>";
            // }
            //
            // await _sendMailService.SendMailAsync(
            //     account.Email,
            //     subject: "Sign-up Verification API - Reset Password",
            //     $@"<h4>Reset Password Email</h4>
            //              {message}"
            // );
            
            
            string frontEndBaseUrl = "http://localhost:3000";
    
            // Construct the URL to the password reset page on your front-end application.
            // The reset token is appended as a query parameter.
            var resetUrl = $"{frontEndBaseUrl}/reset-password?token={account.ResetToken}";

            string message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                        <p>
                            <a 
                                style=""display: inline-block;
                                        padding: 10px 20px;
                                        background-color: #00cc66;
                                        color: #fff;
                                        text-decoration: none;
                                        font-weight: bold;
                                        border: none;
                                        border-radius: 5px; "" 
                                href=""{resetUrl}"">Reset password
                            </a>
                        </p>";

            await _sendMailService.SendMailAsync(
                account.Email,
                subject: "Reset Password",
                message
            );
        }
        
        // private async Task<User> GetDeleteAccount(string id)
        // {
        //     var account = await _db.Users.FindAsync(u => u.Id == id && u.IsDeleted);
        //     if (account == null) throw new KeyNotFoundException("Account not found");
        //
        //     return account;
        // }
        
        public async Task UpgradeToPremium(string userId)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    throw new KeyNotFoundException("User not found");
                }

                // Check if the user is already premium
                if (user.IsPremium)
                {
                    throw new InvalidOperationException("User is already premium");
                }

                // Implement your logic to upgrade the user to premium
                // For example, set the IsPremium property to true
                user.IsPremium = true;

                // Save changes to the database
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error upgrading user to premium: {ex.Message}");
                throw;
            }
        }

        public async Task FollowUser(string followerId, string userId)
        {
            if (userId == followerId)
            {
                throw new ArgumentException("Users cannot follow themselves.");
            }

            var existingFollow = await _db.UserFollowers
                .AnyAsync(uf => uf.UserId == userId && uf.FollowerId == followerId);
            if (!existingFollow)
            {
                var following = new UserFollower
                {
                    UserId = userId,
                    FollowerId = followerId
                };

                _db.UserFollowers.Add(following);
                await _db.SaveChangesAsync();
            }
        }


        public async Task UnfollowUser(string followerId, string userId)
        {
            var following = await _db.UserFollowers
                .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.FollowerId == followerId);

            if (following != null)
            {
                _db.UserFollowers.Remove(following);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<User>> GetFollowers(string userId)
        {
            return await _db.UserFollowers
                .Where(uf => uf.UserId == userId)
                .Select(uf => uf.Follower)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetFollowing(string userId)
        {
            return await _db.UserFollowers
                .Where(uf => uf.FollowerId == userId)
                .Select(uf => uf.User)
                .ToListAsync();
        }
        
        public async Task<List<TutorialDTO>> GetLatestTutorialsByUser(string userId, int count)
        {
            var tutorials = await _mongoDbContext.Tutorials
                .Find(t => t.CreatedById == userId)
                .SortByDescending(t => t.UploadTime)
                .Limit(count)
                .ToListAsync();

            return _mapper.Map<List<TutorialDTO>>(tutorials);
        }
    }
}