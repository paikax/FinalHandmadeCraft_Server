// PayPalController.cs

using System;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.IServices;
using Service.Service;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayPalController : ControllerBase
    {
        private readonly IPayPalService _payPalService;
        private readonly AppDbContext _db;
        private readonly UserService _userService;

        public PayPalController(IPayPalService payPalService, AppDbContext db)
        {
            _payPalService = payPalService;
            _db = db;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder(decimal amount)
        {
            try
            {
                var orderId = await _payPalService.CreateOrder(amount);
                return Ok(new { OrderId = orderId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("capture-payment")]
        public async Task<IActionResult> CapturePayment(string orderId)
        {
            try
            {
                var success = await _payPalService.CaptureOrder(orderId);
                if (success)
                {
                    return Ok(new { Message = "Payment captured successfully." });
                }
                else
                {
                    return BadRequest(new { Message = "Failed to capture payment." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        
        [HttpPost("connect-paypal")]
        public async Task<IActionResult> ConnectPayPal([FromBody] PayPalConnectRequest request)
        {
            // Validate request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Save PayPal information to the database
            var user = await _db.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return NotFound();
            }

            user.PayPalEmail = request.PayPalEmail;
            user.PayPalFirstName = request.PayPalFirstName;
            user.PayPalLastName = request.PayPalLastName;
            user.IsPayPalLinked = true;

            await _db.SaveChangesAsync();

            return Ok(new { user.PayPalEmail, user.PayPalFirstName, user.PayPalLastName });
        }
        
        [HttpPost("send-payment")]
        public async Task<IActionResult> SendPayment(string recipientEmail, decimal amount)
        {
            try
            {
                var success = await _payPalService.SendPayment(recipientEmail, amount);
                if (success)
                {
                    return Ok(new { Message = "Payment sent successfully." });
                }
                else
                {
                    return BadRequest(new { Message = "Failed to send payment." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}