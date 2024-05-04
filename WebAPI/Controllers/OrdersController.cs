using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities.Cart;
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

        [HttpGet("all/{userId}")]
        public async Task<ActionResult<List<OrderDto>>> GetOrders(string userId)
        {
            var orders = await _orderService.GetOrders(userId);
            return Ok(orders);
        }
        
        
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(string id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }


        [HttpPost("{userId}")]
        public async Task<ActionResult<OrderDto>> PostOrder(OrderRequest orderRequest, string userId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdOrder = await _orderService.CreateOrder(orderRequest, userId);
                return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.Id }, createdOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(string id, OrderDto orderDto)
        {
            if (id != orderDto.Id)
            {
                return BadRequest();
            }

            await _orderService.UpdateOrder(id, orderDto);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            await _orderService.DeleteOrder(id);

            return NoContent();
        }
        
        
        // add to cart
        [HttpPost("cart/{userId}")]
        public async Task<IActionResult> AddToCart(string userId, CartItem item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _orderService.AddToCart(userId, item);
                    return Ok("Item added to cart successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("cart/buy/{userId}")]
        public async Task<IActionResult> BuyItemsFromCart(string userId, string buyUserAddress)
        {
            try
            {
                await _orderService.BuyItemsFromCart(userId, buyUserAddress);
                return Ok("Items purchased successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        [HttpPut("cart/{userId}/{productId}")]
        public async Task<IActionResult> UpdateCartItemQuantity(string userId, string productId, int quantity)
        {
            try
            {
                await _orderService.UpdateCartItemQuantity(userId, productId, quantity);
                return Ok("Cart item quantity updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // Remove from cart
        [HttpDelete("cart/{userId}/{productId}")]
        public async Task<IActionResult> RemoveFromCart(string userId, string productId)
        {
            try
            {
                await _orderService.RemoveFromCart(userId, productId);
                return Ok("Item removed from cart successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        [HttpGet("cart/{userId}")]
        public async Task<ActionResult<List<CartItem>>> GetCartItems(string userId)
        {
            try
            {
                var cartItems = await _orderService.GetCartItems(userId);
                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        [HttpDelete("cart/{userId}")]
        public async Task<IActionResult> ClearCart(string userId)
        {
            try
            {
                await _orderService.ClearCart(userId);
                return Ok("Cart cleared successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}