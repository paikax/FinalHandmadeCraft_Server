using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data.Context;
using Data.Entities.Cart;
using Data.Entities.Notification;
using Data.Entities.Order;
using Data.VM;
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
        private readonly IPayPalService _payPalService;

        public OrderService(MongoDbContext mongoDbContext, ISendMailService sendMailService, 
            IPayPalService payPalService,
            IMapper mapper, AppDbContext sqlContext)
        {
            _mongoDbContext = mongoDbContext;
            _sendMailService = sendMailService;
            _mapper = mapper;
            _sqlContext = sqlContext;
            _payPalService = payPalService;
        }
        
        private const decimal CommissionRate = 0.05m;

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
                            ProductName = tutorial.Title, 
                            TutorialImageUrl = tutorial.VideoUrl,
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
                        ProductName = tutorial.Title, 
                        TutorialImageUrl = tutorial.VideoUrl, 
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
            // order.SellerEmail = orderRequest.SellerEmail;
            // order.CreatorId = orderRequest.CreatorId;
            order.BuyerEmail = order.BuyerEmail;

            // Fetch tutorial details for each item in the order and update order properties
            foreach (var item in order.Items)
            {
                var tutorial = await _mongoDbContext.Tutorials.Find(t => t.Id == item.TutorialId).FirstOrDefaultAsync();
                if (tutorial != null)
                {
                    item.Price = tutorial.Price;
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
            // Send emails to sellers for each item in the order
            foreach (var item in order.Items)
            {
                var sellerEmailContent = await GenerateSellerEmailContent(order, item);
                await _sendMailService.SendMail(sellerEmailContent);
            }

            // Send email to the buyer
            var buyerEmailContent = await GenerateBuyerEmailContent(order);
            await _sendMailService.SendMail(buyerEmailContent);
        }


        private async Task<MailContent> GenerateBuyerEmailContent(Order order)
        {
            var subject = "Your order details from HandMadeCraft";
            var body = "<!DOCTYPE html>" +
                       "<html lang=\"en\">" +
                       "<head>" +
                       "<meta charset=\"UTF-8\">" +
                       "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">" +
                       "<title>Order Details</title>" +
                       "<style>" +
                       "body {" +
                       "    font-family: Poppins, sans-serif;" +
                       "    margin: 0;" +
                       "    padding: 0;" +
                       "}" +
                       "div {" +
                       "    margin: 10px;" +
                       "    padding: 20px;" +
                       "    background : linear-gradient(rgba(0,0,0,0.1), rgba(0,0,0,0.8)),url('https://w.wallhaven.cc/full/l8/wallhaven-l8vp7y.jpg');" +
                       "    background-position: center;" +
                       "    background-size: cover;" +
                       "    border-radius: 10px;" +
                       "    color: #fff;" +
                       "    box-shadow: rgba(0, 0, 0, 0.25) 0px 54px 55px, rgba(0, 0, 0, 0.12) 0px -12px 30px, rgba(0, 0, 0, 0.12) 0px 4px 6px, rgba(0, 0, 0, 0.17) 0px 12px 13px, rgba(0, 0, 0, 0.09) 0px -3px 5px;" +
                       "}" +
                       "h1 {" +
                       "    text-align: center;" +
                       "}" +
                       "p {" +
                       "    margin: 0;" +
                       "    padding: 0;" +
                       "}" +
                       "ul {" +
                       "    list-style-type: none;" +
                       "    padding: 0;" +
                       "}" +
                       "li {" +
                       "    margin-bottom: 10px;" +
                       "}" +
                       "</style>" +
                       "</head>" +
                       "<body>" +
                       "<div>" +
                       "<h1>Thank you for your order from HandMadeCraft!</h1>" +
                       "<p>Here are the details of your purchase:</p>" +
                       $"<p><strong>Order ID:</strong> {order.Id}</p>" +
                       $"<p><strong>Order Date:</strong> {order.OrderDate}</p>" +
                       "<h2>Items:</h2>" +
                       "<ul>";

            foreach (var item in order.Items)
            {
                var productName = await GetProductNameFromTutorialId(item.TutorialId);
                body += $"<li>{productName} (Price: {item.Price}, Quantity: {item.Quantity})</li>";
            }

            body += "</ul>" +
                    $"<p><strong>Total Price:</strong> {order.TotalPrice}</p>" +
                    $"<p><strong>Transaction Fee:</strong> {order.TotalPrice * CommissionRate}</p>" +
                    $"<p><strong>Amount after Transaction Fee:</strong> {order.TotalPrice - (order.TotalPrice * CommissionRate)}</p>" +
                    "</div>" +
                    "</body>" +
                    "</html>";

            var buyerEmail = order.BuyerEmail ?? "";
            return new MailContent { To = buyerEmail, Subject = subject, Body = body };
        }

        private async Task<MailContent> GenerateSellerEmailContent(Order order, OrderItem item)
        {
            var subject = "New order received on HandMadeCraft";
            var body = "<h1 style=\"color: #4CAF50;\">You have received a new order on HandMadeCraft!</h1>";
            body += "<p>Here are the details:</p>";
            
            // Append order details to the email body
            body += $"<p><strong>Order ID:</strong> {order.Id}</p>";
            body += $"<p><strong>Order Date:</strong> {order.OrderDate}</p>";
            body += "<h2>Item Details:</h2>";
            var productName = await GetProductNameFromTutorialId(item.TutorialId);
            body += $"<p><strong>Product Name:</strong> {productName}</p>";
            body += $"<p><strong>Price:</strong> {item.Price}</p>";
            body += $"<p><strong>Quantity:</strong> {item.Quantity}</p>";
            body += $"<p><strong>Total Price:</strong> {item.Price * item.Quantity}</p>";
            
            // Include transaction fee information
            var transactionFee = item.Price * item.Quantity * CommissionRate;
            body += $"<p><strong>Transaction Fee:</strong> {transactionFee}</p>";
            body += $"<p><strong>Amount after Transaction Fee:</strong> {item.Price * item.Quantity - transactionFee}</p>";

            // Fetch the tutorial to get the creator's ID
            var tutorial = await _mongoDbContext.Tutorials
                .Find(t => t.Id == item.TutorialId)
                .FirstOrDefaultAsync();

            if (tutorial != null)
            {
                // Fetch the creator's email using the creator's ID from the tutorial
                var creatorEmail = await GetCreatorEmailFromId(tutorial.CreatedById);
                return new MailContent { To = creatorEmail, Subject = subject, Body = body};
            }
            else
            {
                throw new Exception($"Tutorial not found for the TutorialId: {item.TutorialId}");
            }
        }

        
        private async Task<string> GetCreatorEmailFromId(string creatorId)
        {
            // Assuming you have a mechanism to retrieve the creator's email using the creatorId
            var creator = await _sqlContext.Users.FirstOrDefaultAsync(u => u.Id == creatorId);
            if (creator != null)
            {
                return creator.Email;
            }
            else
            {
                throw new Exception($"Creator not found for the CreatorId: {creatorId}");
            }
        }
        
        private async Task<string> GetCreatorPayPalEmailFromId(string creatorId)
        {
            // Assuming you have a mechanism to retrieve the creator's email using the creatorId
            var creator = await _sqlContext.Users.FirstOrDefaultAsync(u => u.Id == creatorId);
            if (creator != null)
            {
                return creator.PayPalEmail;
            }
            else
            {
                throw new Exception($"Creator not found for the CreatorId: {creatorId}");
            }
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
        public async Task<List<CartItemViewModel>> GetCartItems(string userId)
        {
            var shoppingSession = await _mongoDbContext.ShoppingSessions
                .Find(session => session.UserId == userId)
                .FirstOrDefaultAsync();

            if (shoppingSession == null)
            {
                return new List<CartItemViewModel>(); // Return an empty list if the shopping session is not found
            }

            var cartItemsViewModel = new List<CartItemViewModel>();

            foreach (var cartItem in shoppingSession.Items)
            {
                var tutorial = await _mongoDbContext.Tutorials.Find(t => t.Id == cartItem.ProductId).FirstOrDefaultAsync();
                if (tutorial != null)
                {
                    var cartItemViewModel = new CartItemViewModel
                    {
                        ProductId = cartItem.ProductId,
                        ProductName = tutorial.Title,
                        Price = cartItem.Price,
                        Quantity = cartItem.Quantity,
                        ImageUrl = tutorial.VideoUrl
                    };
                    cartItemsViewModel.Add(cartItemViewModel);
                }
            }

            return cartItemsViewModel;
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
                    var filter = Builders<Cart>.Filter.Eq(session => session.UserId, userId);
                    var update = Builders<Cart>.Update.Set(session => session.Items, shoppingSession.Items);
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
                        shoppingSession = new Cart
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
                                Id = ObjectId.GenerateNewId().ToString(),
                                TutorialId = tutorial.Id,
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
                    
                    // Send payments to sellers for each item in the order
                    foreach (var item in orderItems)
                    {
                        // Fetch the tutorial to get the creator's ID
                        var tutorial = await _mongoDbContext.Tutorials
                            .Find(t => t.Id == item.TutorialId)
                            .FirstOrDefaultAsync();

                        if (tutorial != null)
                        {
                            // Fetch the creator's email using the creator's ID from the tutorial
                            var creatorPayPalEmailEmail = await GetCreatorPayPalEmailFromId(tutorial.CreatedById);
                            
                            // Send payment to the seller
                            var success = await _payPalService.SendPayment(creatorPayPalEmailEmail, item.Price * item.Quantity);
                            
                            if (!success)
                            {
                                // Handle payment failure
                                // You can log the failure or take appropriate action
                            }
                        }
                        else
                        {
                            throw new Exception($"Tutorial not found for the TutorialId: {item.TutorialId}");
                        }
                    }

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
                var filter = Builders<Cart>.Filter.Eq(session => session.UserId, userId);
                var update = Builders<Cart>.Update.Set(session => session.Items, cartItems);
                var options = new FindOneAndUpdateOptions<Cart>
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
            var filter = Builders<Cart>.Filter.Eq(session => session.UserId, userId);
            var update = Builders<Cart>.Update.PullFilter(session => session.Items, item => item.ProductId == productId);
            var options = new FindOneAndUpdateOptions<Cart>
            {
                ReturnDocument = ReturnDocument.After
            };

            await _mongoDbContext.ShoppingSessions.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task UpdateCartItemQuantity(string userId, string productId, int quantity)
        {
            var filter = Builders<Cart>.Filter.And(
                Builders<Cart>.Filter.Eq(session => session.UserId, userId),
                Builders<Cart>.Filter.Eq("Items.ProductId", productId)
            );
            var update = Builders<Cart>.Update.Set("Items.$.Quantity", quantity);
            var options = new FindOneAndUpdateOptions<Cart>
            {
                ReturnDocument = ReturnDocument.After
            };

            await _mongoDbContext.ShoppingSessions.FindOneAndUpdateAsync(filter, update, options);
        }
        
        public async Task ClearCart(string userId)
        {
            await _mongoDbContext.ShoppingSessions.DeleteOneAsync(session => session.UserId == userId);
        }
        
        private async Task<List<string>> GetSellerEmails(List<OrderItem> orderItems)
        {
            var sellerEmails = new List<string>();

            foreach (var item in orderItems)
            {
                var tutorial = await _mongoDbContext.Tutorials.Find(t => t.Id == item.TutorialId).FirstOrDefaultAsync();
                if (tutorial != null)
                {
                    // var creatorEmail = tutorial.CreatorEmail;
                    // if (!sellerEmails.Contains(creatorEmail))
                    // {
                    //     sellerEmails.Add(creatorEmail); // Add seller email to the list if it's not already present
                    // }
                }
            }

            return sellerEmails;
        }
    }
}