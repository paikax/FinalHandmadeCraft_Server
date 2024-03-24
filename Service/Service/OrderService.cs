using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data.Context;
using Data.Entities.Cart;
using Data.Entities.Notification;
using Data.Entities.Order;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using Service.IServices;
using Service.Utils;


namespace Service.Service
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _sqlContext;
        private readonly MongoDbContext _mongoDbContext;
        private readonly ISendMailService _sendMailService;
        private readonly IMapper _mapper;

        public OrderService(MongoDbContext mongoDbContext, ISendMailService sendMailService, 
            IMapper mapper, AppDbContext sqlContext)
        {
            _mongoDbContext = mongoDbContext;
            _sendMailService = sendMailService;
            _mapper = mapper;
            _sqlContext = sqlContext;
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
                Message = $"You have received a new order with ({order.Items.FirstOrDefault()?.ProductName})",
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
            var buyerEmailContent = GenerateBuyerEmailContent(order);
            await _sendMailService.SendMail(sellerEmailContent);
            await _sendMailService.SendMail(buyerEmailContent);
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

            var buyerEmail = order.BuyerEmail ?? "";
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

            var sellerEmail = order.SellerEmail ?? "";
            // // for demo send mail after order
            // var sellerEmail = "paikax2060@gmail.com";
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
        
        // Shopping cart
        public async Task<List<CartItem>> GetCartItems(string userId)
        {
            var shoppingSession = await _mongoDbContext.ShoppingSessions
                .Find(session => session.UserId == userId)
                .FirstOrDefaultAsync();

            return shoppingSession?.Items ?? new List<CartItem>();
        }

        public async Task AddToCart(string userId, CartItem cartItem)
        {
            // Fetch tutorial details based on product ID
            var tutorial = await _mongoDbContext.Tutorials
                .Find(t => t.Id == cartItem.ProductId)
                .FirstOrDefaultAsync();

            if (tutorial != null)
            {
                // Create a cart item with tutorial details
                var itemToAdd = new CartItem
                {
                    ProductId = tutorial.Id,
                    Price = tutorial.Price,
                    Quantity = cartItem.Quantity
                };

                // Add the item to the user's shopping cart
                var filter = Builders<ShoppingSession>.Filter.Eq(session => session.UserId, userId);
                var update = Builders<ShoppingSession>.Update.Push(session => session.Items, itemToAdd);
                var options = new FindOneAndUpdateOptions<ShoppingSession>
                {
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After
                };

                await _mongoDbContext.ShoppingSessions.FindOneAndUpdateAsync(filter, update, options);
            }
            else
            {
                throw new Exception("Tutorial not found for the provided ID.");
            }
        }

        public async Task<OrderDto> BuyItemsFromCart(string userId)
        {
            var buyerUser = await _sqlContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (buyerUser == null)
            {
                throw new Exception("User not found.");
            }
            var cartItems = await GetCartItems(userId);
            if (cartItems.Any())
            {
                var orderItems = new List<OrderItem>();

                // Iterate through cart items to create order items
                foreach (var cartItem in cartItems)
                {
                    // Fetch tutorial details based on product ID
                    var tutorial = await _mongoDbContext.Tutorials
                        .Find(t => t.Id == cartItem.ProductId)
                        .FirstOrDefaultAsync();

                    if (tutorial != null)
                    {
                        orderItems.Add(new OrderItem
                        {
                            Id = tutorial.Id,
                            ProductName = tutorial.Title, // Include product name in order item
                            TutorialImageUrl = tutorial.VideoUrl,
                            Price = tutorial.Price,
                            Quantity = cartItem.Quantity
                        });
                    }
                    else
                    {
                        throw new Exception($"Tutorial not found for the product ID: {cartItem.ProductId}");
                    }
                }

                // Calculate total price
                decimal totalPrice = orderItems.Sum(item => item.Price * item.Quantity);

                // Create an order request
                var orderRequest = new OrderRequest
                {
                    UserId = userId,
                    Items = orderItems,
                    TotalPrice = totalPrice,
                    OrderDate = DateTime.Now,
                    LastUpdated = DateTime.Now,
                    BuyerEmail = buyerUser.Email
                };

                // Create the order
                var order = await CreateOrder(orderRequest, userId);

                // Clear the shopping cart after creating the order
                await UpdateCart(userId, new List<CartItem>());

                return order;
            }
            else
            {
                throw new Exception("Cart is empty. Cannot buy items.");
            }
        }
        

        public async Task UpdateCart(string userId, List<CartItem> cartItems)
        {
            var filter = Builders<ShoppingSession>.Filter.Eq(session => session.UserId, userId);
            var update = Builders<ShoppingSession>.Update.Set(session => session.Items, cartItems);
            var options = new FindOneAndUpdateOptions<ShoppingSession>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            await _mongoDbContext.ShoppingSessions.FindOneAndUpdateAsync(filter, update, options);
        }

    }
}