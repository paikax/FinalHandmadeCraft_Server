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
using SendGrid.Helpers.Errors.Model;
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
    
            var orderDtos = new List<OrderDto>();

            foreach (var order in orders)
            {
                var orderDto = new OrderDto
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    Address = order.Address,
                    OrderDate = order.OrderDate,
                    LastUpdated = order.LastUpdated,
                    TotalPrice = order.TotalPrice,
                    BuyerEmail = order.BuyerEmail,
                    Items = new List<OrderItemDto>()
                };

                foreach (var orderItem in order.Items)
                {
                    var tutorial = await _mongoDbContext.Tutorials.Find(t => t.Id == orderItem.TutorialId).FirstOrDefaultAsync();
                    if (tutorial != null)
                    {
                        var orderItemDto = new OrderItemDto
                        {
                            ProductId = orderItem.Id,
                            ProductName = tutorial.Title, // Assuming the name of the tutorial is stored in the 'Name' property
                            TutorialImageUrl = tutorial.VideoUrl, // Assuming the URL of the tutorial image is stored in the 'ImageUrl' property
                            Price = orderItem.Price,
                            Quantity = orderItem.Quantity
                        };
                        orderDto.Items.Add(orderItemDto);
                    }
                    else
                    {
                        throw new Exception($"Tutorial not found for the product ID: {orderItem.TutorialId}");
                    }
                }

                orderDtos.Add(orderDto);
            }

            return orderDtos;
        }

        
        public async Task<OrderDto> GetOrderById(string id)
        {
            var order = await _mongoDbContext.Orders.Find(o => o.Id == id).FirstOrDefaultAsync();
            if (order == null)
            {
                throw new NotFoundException("Order not found");
            }

            var orderDto = new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                Address = order.Address,
                OrderDate = order.OrderDate,
                LastUpdated = order.LastUpdated,
                TotalPrice = order.TotalPrice,
                BuyerEmail = order.BuyerEmail,
                Items = new List<OrderItemDto>()
            };

            foreach (var orderItem in order.Items)
            {
                var tutorial = await _mongoDbContext.Tutorials.Find(t => t.Id == orderItem.TutorialId).FirstOrDefaultAsync();
                if (tutorial != null)
                {
                    var orderItemDto = new OrderItemDto
                    {
                        ProductId = orderItem.Id,
                        ProductName = tutorial.Title, // Assuming the name of the tutorial is stored in the 'Name' property
                        TutorialImageUrl = tutorial.VideoUrl, // Assuming the URL of the tutorial image is stored in the 'ImageUrl' property
                        Price = orderItem.Price,
                        Quantity = orderItem.Quantity
                    };
                    orderDto.Items.Add(orderItemDto);
                }
                else
                {
                    throw new Exception($"Tutorial not found for the product ID: {orderItem.TutorialId}");
                }
            }

            return orderDto;
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
                    // item.ProductName = tutorial.Title;
                    // item.TutorialImageUrl = tutorial.VideoUrl;
                    item.Price = tutorial.Price;
                    
                    // Get the creatorId of the tutorial
                    // item.CreatorId = tutorial.CreatedById;
                }
            }

            // var notification = new Notification()
            // { 
            //     BuyerId = userId,
            //     SellerId = order.Items.FirstOrDefault()?.CreatorId ?? "",
            //     Title = "New Order Received",
            //     Message = $"You have received a new order with ({order.Items.FirstOrDefault()?.ProductName})",
            //     CreatedAt = DateTime.Now
            // };
            await _mongoDbContext.Orders.InsertOneAsync(order);
            await SendEmails(order);
                // await _mongoDbContext.Notifications.InsertOneAsync(notification);
                
            return _mapper.Map<OrderDto>(order);
        }
        
        private async Task<string> GetProductNameFromTutorialId(string tutorialId)
        {
            var tutorial = await _mongoDbContext.Tutorials
                .Find(t => t.Id == tutorialId)
                .FirstOrDefaultAsync();
    
            return tutorial != null ? tutorial.Title : "Unknown Title";
        }
        
        private async Task SendEmails(Order order)
        {
            // Generate email content
            var sellerEmailContent = await GenerateSellerEmailContent(order);
            var buyerEmailContent = await GenerateBuyerEmailContent(order);
    
            // Send emails
            await _sendMailService.SendMail(sellerEmailContent);
            await _sendMailService.SendMail(buyerEmailContent);
        }


        private async Task<MailContent> GenerateBuyerEmailContent(Order order)
        {
            var subject = "Your order bill detail";
            var body = "Thank you for your order! Here are the details of your purchase:\n\n";

            // Append order details to the email body
            body += $"Order ID: {order.Id}\n";
            body += $"Order Date: {order.OrderDate}\n";
            body += "Items:\n";
            foreach (var item in order.Items)
            {
                var productName = await GetProductNameFromTutorialId(item.TutorialId);
                body += $"- {productName} (Price: {item.Price}, Quantity: {item.Quantity})\n";
            }
            body += $"\nTotal Price: {order.TotalPrice}\n";

            var buyerEmail = order.BuyerEmail ?? "";
            return new MailContent { To = buyerEmail, Subject = subject, Body = body };
        }

        private async Task<MailContent> GenerateSellerEmailContent(Order order)
        {
            var subject = "New Order Received";
            var body = "You have received a new order! Here are the details:\n\n";

            // Append order details to the email body
            body += $"Order ID: {order.Id}\n";
            body += $"Order Date: {order.OrderDate}\n";
            body += "Items:\n";
            foreach (var item in order.Items)
            {
                var productName = await GetProductNameFromTutorialId(item.TutorialId);
                body += $"- {productName} (Price: {item.Price}, Quantity: {item.Quantity})\n";
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
            var tutorial = await _mongoDbContext.Tutorials
                .Find(t => t.Id == cartItem.ProductId)
                .FirstOrDefaultAsync();

            if (tutorial != null)
            {
                var shoppingSession = await _mongoDbContext.ShoppingSessions
                    .Find(session => session.UserId == userId)
                    .FirstOrDefaultAsync();

                // Check if the item already exists in the cart
                var existingItem = shoppingSession?.Items.FirstOrDefault(item => item.ProductId == cartItem.ProductId);
                if (existingItem != null)
                {
                    // Update the quantity of the existing item
                    existingItem.Quantity += cartItem.Quantity;
                    var filter = Builders<ShoppingSession>.Filter.Eq(session => session.UserId, userId);
                    var update = Builders<ShoppingSession>.Update.Set(session => session.Items, shoppingSession.Items);
                    await _mongoDbContext.ShoppingSessions.UpdateOneAsync(filter, update);
                }
                else
                {
                    // Create a new cart item with tutorial details
                    var itemToAdd = new CartItem
                    {
                        ProductId = tutorial.Id,
                        Price = tutorial.Price,
                        Quantity = cartItem.Quantity
                    };

                    // Add the item to the user's shopping cart
                    if (shoppingSession == null)
                    {
                        shoppingSession = new ShoppingSession
                        {
                            UserId = userId,
                            Items = new List<CartItem> { itemToAdd }
                        };
                        await _mongoDbContext.ShoppingSessions.InsertOneAsync(shoppingSession);
                    }
                    else
                    {
                        // Add the new item to the existing cart items
                        shoppingSession.Items.Add(itemToAdd);
                        await _mongoDbContext.ShoppingSessions.ReplaceOneAsync(session => session.UserId == userId, shoppingSession);
                    }
                }
            }
            else
            {
                throw new Exception("Tutorial not found for the provided ID.");
            }
        }
        

        public async Task<OrderDto> BuyItemsFromCart(string userId, string address)
        {
            try
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
                                Price = tutorial.Price,
                                Quantity = cartItem.Quantity,
                            });
                            
                        }
                        else
                        {
                            throw new Exception($"Tutorial not found for the product ID: {cartItem.ProductId}");
                        }
                    }
                    
                    if (string.IsNullOrEmpty(address))
                    {
                        throw new Exception("No address found.");
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
                        BuyerEmail = buyerUser.Email,
                        Address = address,
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
            catch (Exception ex)
            {
                throw new Exception($"Error buying items from cart: {ex.Message}");
            }
        }

        

        public async Task UpdateCart(string userId, List<CartItem> cartItems)
        {
            try
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
            catch (Exception ex)
            {
                // Handle the exception appropriately, log or throw it
                Console.WriteLine($"Error updating cart for user {userId}: {ex.Message}");
                throw;
            }
        }


        public async Task RemoveFromCart(string userId, string productId)
        {
            var filter = Builders<ShoppingSession>.Filter.Eq(session => session.UserId, userId);
            var update = Builders<ShoppingSession>.Update.PullFilter(session => session.Items, item => item.ProductId == productId);
            var options = new FindOneAndUpdateOptions<ShoppingSession>
            {
                ReturnDocument = ReturnDocument.After
            };

            await _mongoDbContext.ShoppingSessions.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task UpdateCartItemQuantity(string userId, string productId, int quantity)
        {
            var filter = Builders<ShoppingSession>.Filter.And(
                Builders<ShoppingSession>.Filter.Eq(session => session.UserId, userId),
                Builders<ShoppingSession>.Filter.Eq("Items.ProductId", productId)
            );
            var update = Builders<ShoppingSession>.Update.Set("Items.$.Quantity", quantity);
            var options = new FindOneAndUpdateOptions<ShoppingSession>
            {
                ReturnDocument = ReturnDocument.After
            };

            await _mongoDbContext.ShoppingSessions.FindOneAndUpdateAsync(filter, update, options);
        }
        
        public async Task ClearCart(string userId)
        {
            await _mongoDbContext.ShoppingSessions.DeleteOneAsync(session => session.UserId == userId);
        }

    }
}