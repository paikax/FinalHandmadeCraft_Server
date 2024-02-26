﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Data.Context;
using Data.Entities.Order;
using MongoDB.Driver;
using Service.IServices;


namespace Service.Service
{
    public class OrderService : IOrderService
    {
        private readonly MongoDbContext _mongoDbContext;
        private readonly IMapper _mapper;

        public OrderService(MongoDbContext mongoDbContext, IMapper mapper)
        {
            _mongoDbContext = mongoDbContext;
            _mapper = mapper;
        }

        public async Task<List<OrderDto>> GetOrders(string userId)
        {
            var orders = await _mongoDbContext.Orders.Find(o => o.UserId == userId).ToListAsync();
            return _mapper.Map<List<OrderDto>>(orders);
        }

        public async Task<OrderDto> GetOrder(string id)
        {
            var order = await _mongoDbContext.Orders.Find(o => o.Id == id).FirstOrDefaultAsync();
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CreateOrder(OrderRequest orderRequest, string userId)
        {
            var order = _mapper.Map<Order>(orderRequest);
            order.UserId = userId;
            await _mongoDbContext.Orders.InsertOneAsync(order);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task UpdateOrder(string id, OrderDto orderDto)
        {
            var order = _mapper.Map<Order>(orderDto);
            await _mongoDbContext.Orders.ReplaceOneAsync(o => o.Id == id, order);
        }

        public async Task DeleteOrder(string id)
        {
            await _mongoDbContext.Orders.DeleteOneAsync(o => o.Id == id);
        }
    }
}