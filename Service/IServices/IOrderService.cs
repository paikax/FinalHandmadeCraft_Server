using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities.Order;

namespace Service.IServices
{
    public interface IOrderService
    {
        Task<List<Order>> GetOrders(string userId);
        Task<Order> GetOrder(string id);
        Task<Order> CreateOrder(Order order, string userId);
        Task UpdateOrder(string id, Order order);
        Task DeleteOrder(string id);
    }
}