using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities.Cart;
using Data.Entities.Order;

namespace Service.IServices
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetOrders(string userId);
        Task<OrderDto> GetOrderById(string id);
        Task<OrderDto> CreateOrder(OrderRequest orderRequest, string userId);
        Task UpdateOrder(string id, OrderDto orderDto);
        Task DeleteOrder(string id);
        
        Task<List<CartItem>> GetCartItems(string userId);
        Task AddToCart(string userId, CartItem cartItem);
        Task RemoveFromCart(string userId, string productId);
        Task UpdateCartItemQuantity(string userId, string productId, int quantity);
        Task<OrderDto> BuyItemsFromCart(string userId, string address);
        Task UpdateCart(string userId, List<CartItem> cartItems);
        Task ClearCart(string userId);
    }
}