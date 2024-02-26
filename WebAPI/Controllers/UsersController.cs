using System;
using System.Threading.Tasks;
using AutoMapper;
using Data.Entities.User;
using Data.ViewModels.User;
using Microsoft.AspNetCore.Mvc;
using Service.IServices;
using Microsoft.AspNetCore.Http;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _mapper = mapper;
            _userService = userService;
        }
        
        [HttpGet("check-email")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            var emailExists = await _userService.EmailExists(email);
            return Ok(new { EmailExists = emailExists });
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            try
            {
                var user = _mapper.Map<User>(model);
                await _userService.Register(user, model.Password, Request.Headers["origin"]);
                return Ok(new { Message = "User registered successfully. Verification email has been sent." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            try
            {
                var user = await _userService.VerifyEmail(token);
                return Ok(new { Message = "User email verified successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        
        
        
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticationRequest model)
        {
            try
            {
                var response = await _userService.Authenticate(model, IpAddress());
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            await _userService.ForgotPassword(model, Request.Headers["origin"]);
            return Ok(new { message = "Please check your email for password reset instructions" });
        }
        
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
            await _userService.ResetPassword(model);
            return Ok(new { message = "Password reset successful, now you can login" });
            
        }
        
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(string refreshToken)  
        {
            await _userService.Logout(refreshToken);
            return Ok(new { message = "Logged out successfully" });
        }
        
        

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAll();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var user = await _userService.GetById(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateRequest updateRequest)
        {
            try
            {
                await _userService.Update(id, updateRequest);
                return Ok(new { Message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                await _userService.Delete(id);
                return Ok(new { Message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        // helper method
        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions()
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
        
        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-for"))
                return Request.Headers["X-Forwarded-For"];
            else
            {
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
        }
        
        [HttpPost("{userId}/upgrade-to-premium")]
        public async Task<IActionResult> UpgradeToPremium(string userId)
        {
            try
            {
                // Pass userId to the service method for upgrading to premium
                await _userService.UpgradeToPremium(userId);
                return Ok(new { Message = "User upgraded to premium successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("{userId}/follow/{followerId}")]
        public async Task<IActionResult> Follow(string userId, string followerId)
        {
            if (userId == followerId)
            {
                return BadRequest("Users cannot follow themselves.");
            }

            try
            {
                await _userService.FollowUser(followerId, userId);
                return Ok(new { Message = "Followed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        [HttpPost("{userId}/unfollow/{followerId}")]
        public async Task<IActionResult> Unfollow(string userId, string followerId)
        {
            await _userService.UnfollowUser(followerId, userId);
            return Ok();
        }

        [HttpGet("{userId}/followers")]
        public async Task<IActionResult> GetFollowers(string userId)
        {
            var followers = await _userService.GetFollowers(userId);
            return Ok(followers);
        }

        [HttpGet("{userId}/following")]
        public async Task<IActionResult> GetFollowing(string userId)
        {
            var following = await _userService.GetFollowing(userId);
            return Ok(following);
        }
        
        [HttpGet("{userId}/isFollowing/{followerId}")]
        public async Task<IActionResult> IsFollowing(string userId, string followerId)
        {
            var isFollowing = await _userService.IsFollowing(userId, followerId);
            return Ok(isFollowing);
        }
        
        [HttpGet("{userId}/latest-tutorials")]
        public async Task<IActionResult> GetLatestUserTutorials(string userId, int count = 4)
        {
            try
            {
                var tutorials = await _userService.GetLatestTutorialsByUser(userId, count);
                return Ok(tutorials);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        
    }
}