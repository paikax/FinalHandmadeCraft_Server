using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.IServices;
using WebAPI.Controllers;
using Xunit;

namespace WebAPI.UnitTests
{
    public class PayPalControllerTests
    {
        [Fact]
        public async Task CreateOrder_ReturnsOkResult()
        {
            // Arrange
            decimal amount = 100.0m;
            var orderId = Guid.NewGuid().ToString();
            var payPalServiceMock = new Mock<IPayPalService>();
            payPalServiceMock.Setup(service => service.CreateOrder(amount))
                             .ReturnsAsync(orderId);

            var controller = new PayPalController(payPalServiceMock.Object, null);

            // Act
            var result = await controller.CreateOrder(amount);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(orderId, okResult.Value.GetType().GetProperty("OrderId").GetValue(okResult.Value));
        }

        [Fact]
        public async Task CapturePayment_Success_ReturnsOkResult()
        {
            // Arrange
            var orderId = Guid.NewGuid().ToString();
            var payPalServiceMock = new Mock<IPayPalService>();
            payPalServiceMock.Setup(service => service.CaptureOrder(orderId))
                             .ReturnsAsync(true);

            var controller = new PayPalController(payPalServiceMock.Object, null);

            // Act
            var result = await controller.CapturePayment(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Payment captured successfully.",
                okResult.Value.GetType().GetProperty("Message").GetValue(okResult.Value));
        }

        [Fact]
        public async Task CapturePayment_Failure_ReturnsBadRequestResult()
        {
            // Arrange
            var orderId = Guid.NewGuid().ToString();
            var payPalServiceMock = new Mock<IPayPalService>();
            payPalServiceMock.Setup(service => service.CaptureOrder(orderId))
                             .ReturnsAsync(false);

            var controller = new PayPalController(payPalServiceMock.Object, null);

            // Act
            var result = await controller.CapturePayment(orderId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to capture payment.", 
                badRequestResult.Value.GetType().GetProperty("Message").GetValue(badRequestResult.Value));
        }
    }
}