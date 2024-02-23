using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities.Order;
using MongoDB.Driver;
using Service.IServices;


namespace Service.Service
{


    public class OrderService : IOrderService
    {
        private readonly MongoDbContext _mongoDbContext;

        public OrderService(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }

        public async Task<List<Order>> GetOrders(string userId)
        {
            var orders = await _mongoDbContext.Orders.Find(o => o.UserId == userId).ToListAsync();
            return orders;
        }

        public async Task<Order> GetOrder(string id)
        {
            var order = await _mongoDbContext.Orders.Find(o => o.Id == id).FirstOrDefaultAsync();
            return order;
        }

        public async Task<Order> CreateOrder(Order order, string userId)
        {
            order.UserId = userId;
            await _mongoDbContext.Orders.InsertOneAsync(order);
            return order;
        }

        public async Task UpdateOrder(string id, Order order)
        {
            await _mongoDbContext.Orders.ReplaceOneAsync(o => o.Id == id, order);
        }

        public async Task DeleteOrder(string id)
        {
            await _mongoDbContext.Orders.DeleteOneAsync(o => o.Id == id);
        }
    }

}