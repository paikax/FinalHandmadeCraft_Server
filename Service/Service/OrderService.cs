using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data.Context;
using Data.Entities.Notification;
using Data.Entities.Order;
using MongoDB.Driver;
using Service.IServices;
using Service.Utils;


namespace Service.Service
{
    public class OrderService : IOrderService
    {
        private readonly MongoDbContext _mongoDbContext;
        private readonly ISendMailService _sendMailService;
        private readonly IMapper _mapper;

        public OrderService(MongoDbContext mongoDbContext, ISendMailService sendMailService, IMapper mapper)
        {
            _mongoDbContext = mongoDbContext;
            _sendMailService = sendMailService;
            _mapper = mapper;
        }

        public async Task<List<OrderDto>> GetOrders(string userId)
        {
            var orders = await _mongoDbContext.Orders.Find(o => o.UserId == userId).ToListAsync();
            return _mapper.Map<List<OrderDto>>(orders);
        }
        
        public async Task<OrderDto> GetOrderById(string id)
        {
            var order = await _mongoDbContext.Orders.Find(o => o.Id == id).FirstOrDefaultAsync();
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CreateOrder(OrderRequest orderRequest, string userId)
        {
            var order = _mapper.Map<Order>(orderRequest);
            order.UserId = userId;
            order.OrderDate = DateTime.Now;
            order.SellerEmail = orderRequest.SellerEmail;
            order.CreatorId = orderRequest.CreatorId;
            order.BuyerEmail = order.BuyerEmail;

            // Fetch tutorial details for each item in the order and update order properties
            foreach (var item in order.Items)
            {
                var tutorial = await _mongoDbContext.Tutorials.Find(t => t.Id == item.TutorialId).FirstOrDefaultAsync();
                if (tutorial != null)
                {
                    item.ProductName = tutorial.Title;
                    item.TutorialImageUrl = tutorial.VideoUrl;
                    item.Price = tutorial.Price;
                    
                    // Get the creatorId of the tutorial
                    item.CreatorId = tutorial.CreatedById;
                }
            }

            var notification = new Notification()
            { 
                BuyerId = userId,
                SellerId = order.Items.FirstOrDefault()?.CreatorId ?? "",
                Title = "New Order Received",
                Message = $"You have received a new order with ID: {order.Id}",
                CreatedAt = DateTime.Now
            };
            await _mongoDbContext.Orders.InsertOneAsync(order);
            await SendEmails(order);
                await _mongoDbContext.Notifications.InsertOneAsync(notification);
                
            return _mapper.Map<OrderDto>(order);
        }
        
        private async Task SendEmails(Order order)
        {
            // var buyerEmailContent = GenerateBuyerEmailContent(order);
            // await _sendMailService.SendMail(buyerEmailContent);
            
            var sellerEmailContent = GenerateSellerEmailContent(order);
            await _sendMailService.SendMail(sellerEmailContent);
        }

        private MailContent GenerateBuyerEmailContent(Order order)
        {
            var subject = "Your order bill detail";
            var body = "Thank you for your order! Here are the details of your purchase:\n\n";

            // Append order details to the email body
            body += $"Order ID: {order.Id}\n";
            body += $"Order Date: {order.OrderDate}\n";
            body += "Items:\n";
            foreach (var item in order.Items)
            {
                body += $"- {item.ProductName} (Price: {item.Price}, Quantity: {item.Quantity})\n";
            }
            body += $"\nTotal Price: {order.TotalPrice}\n";

            var buyerEmail = order.BuyerEmail;
            return new MailContent { To = buyerEmail, Subject = subject, Body = body };
        }

        private MailContent GenerateSellerEmailContent(Order order)
        {
            var subject = "New Order Received";
            var body = "You have received a new order! Here are the details:\n\n";

            // Append order details to the email body
            body += $"Order ID: {order.Id}\n";
            body += $"Order Date: {order.OrderDate}\n";
            body += "Items:\n";
            foreach (var item in order.Items)
            {
                body += $"- {item.ProductName} (Price: {item.Price}, Quantity: {item.Quantity})\n";
            }
            body += $"\nTotal Price: {order.TotalPrice}\n";

            // var sellerEmail = order.SellerEmail;
            // for demo send mail after order
            var sellerEmail = "paikax2060@gmail.com";
            return new MailContent { To = sellerEmail, Subject = subject, Body = body };
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