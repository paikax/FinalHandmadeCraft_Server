using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities.Order;
using Service.IServices;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<List<Order>>> GetOrders(string userId)
        {
            var orders = await _orderService.GetOrders(userId);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(string id)
        {
            var order = await _orderService.GetOrder(id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult<Order>> PostOrder(OrderRequest orderRequest, string userId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Optionally, map OrderRequest to Order
                var order = new Order
                {
                    UserId = userId,
                    Items = orderRequest.Items,
                    Address = orderRequest.Address
                };

                var createdOrder = await _orderService.CreateOrder(order, userId);
                return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.Id }, createdOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(string id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            await _orderService.UpdateOrder(id, order);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            await _orderService.DeleteOrder(id);

            return NoContent();
        }
    }
}