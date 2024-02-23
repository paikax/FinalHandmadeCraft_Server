// PayPalController.cs

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service.Service;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayPalController : ControllerBase
    {
        private readonly PayPalService _payPalService;

        public PayPalController(PayPalService payPalService)
        {
            _payPalService = payPalService;
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
    }
}