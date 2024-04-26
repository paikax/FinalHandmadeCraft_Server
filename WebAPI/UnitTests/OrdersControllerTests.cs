using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities.Order;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.IServices;
using WebAPI.Controllers;
using Xunit;

namespace WebAPI.UnitTests
{
    public class OrdersControllerTests
    {
        [Fact]
        public async Task GetOrders_ReturnsOkResult()
        {
            // Arrange
            var userId = "06084eaf-a70f-42b3-9bd7-40a9e6b955f1";
            var orders = new List<OrderDto> { new OrderDto(), new OrderDto() }; 
            var orderServiceMock = new Mock<IOrderService>();
            orderServiceMock.Setup(repo => repo.GetOrders(userId)).ReturnsAsync(orders);
            var controller = new OrdersController(orderServiceMock.Object);

            // Act
            var result = await controller.GetOrders(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedOrders = Assert.IsAssignableFrom<List<OrderDto>>(okResult.Value);
            Assert.Equal(2, returnedOrders.Count); 
        }
    }
}