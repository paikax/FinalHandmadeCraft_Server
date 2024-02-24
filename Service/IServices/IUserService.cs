using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities.User;
using Data.ViewModels.User;
using Microsoft.AspNetCore.Mvc;

namespace Service.IServices
{
    public interface IUserService
    {
        Task<AuthenticationResponse> Authenticate(AuthenticationRequest model, string ipAddress);
        Task<AuthenticationResponse> RefreshToken(string token, string ipAddress);
        Task<bool> RevokeToken(string token, string ipAddress);
        Task ValidateResetToken(ValidateResetTokenRequest model);
        Task Activate(string id);
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(string id);
        Task Register(User model, string password, string origin);
        Task ForgotPassword(ForgotPasswordRequest model, string origin);
        Task ResetPassword(ResetPasswordRequest model);
        Task Update(string id, UpdateRequest model);
        Task Delete(string id);
        Task<User> VerifyEmail(string token);
        Task<bool> EmailExists(string email);
        
        Task Logout(string refreshToken);
        Task UpgradeToPremium(string userId);
        Task FollowUser(string followerId, string userId);
        Task UnfollowUser(string followerId, string userId);
        Task<IEnumerable<User>> GetFollowers(string userId);
        Task<IEnumerable<User>> GetFollowing(string userId);
        public Task<bool> IsFollowing(string followerId, string userId);
        
    }
}