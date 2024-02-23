using System.Threading.Tasks;
using Data.Entities.User;
using Microsoft.AspNetCore.Mvc;
using Service.IServices;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentSetupController : ControllerBase
    {
        private readonly IPaymentSetupService _paymentSetupService;

        public PaymentSetupController(IPaymentSetupService paymentSetupService)
        {
            _paymentSetupService = paymentSetupService;
        }

        [HttpPost("setup-paypal/{userId}")]
        public async Task<IActionResult> SetupPayPal(string userId, PayPalSetupRequest request)
        {
            var result = await _paymentSetupService.SetupPayPal(userId, request);
            if (result)
            {
                // Optionally return the user's PayPal email and name in the response
                var paypalInfo = await _paymentSetupService.GetUserPayPalInfo(userId);
                if (paypalInfo != null)
                {
                    return Ok(new { Message = "PayPal setup successful.", PayPalInfo = paypalInfo });
                }
                else
                {
                    return NotFound("User not found or PayPal info not available.");
                }
            }

            return NotFound("User not found or unable to set up PayPal.");
        }
    }
}