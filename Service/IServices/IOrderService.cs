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
        Task<OrderDto> BuyItemsFromCart(string userId);
        Task UpdateCart(string userId, List<CartItem> cartItems);
    }
}