using System.Collections.Generic;
using System.Threading.Tasks;
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
    }
}