// PayPalService.cs

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Data.Entities.Order;
using Data.Entities.User;
using Microsoft.Extensions.Configuration;
using Service.IServices;


namespace Service.Service
{
    public class PayPalService : IPayPalService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly HttpClient _httpClient;

        public PayPalService(IConfiguration configuration, HttpClient httpClient)
        {
            _clientId = configuration["PayPal:ClientId"];
            _clientSecret = configuration["PayPal:ClientSecret"];
            _httpClient = httpClient;
        }

        public async Task<string> CreateOrder(decimal amount)
        {
            var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

            var payload = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new
                    {
                        amount = new
                        {
                            currency_code = "USD",
                            value = amount.ToString("0.00")
                        }
                    }
                }
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.sandbox.paypal.com/v2/checkout/orders", content);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var order = JsonSerializer.Deserialize<PayPalOrderResponse>(responseContent);
            return order.id;
        }

        public async Task<bool> CaptureOrder(string orderId)
        {
            var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

            var response = await _httpClient.PostAsync($"https://api.sandbox.paypal.com/v2/checkout/orders/{orderId}/capture", null);

            response.EnsureSuccessStatusCode();

            return response.IsSuccessStatusCode;
        }
        
        public async Task<bool> SendPayment(string recipientEmail, decimal amount)
        {
            try
            {
                var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}"));
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

                var payload = new
                {
                    sender_batch_header = new
                    {
                        sender_batch_id = Guid.NewGuid().ToString(),
                        email_subject = "Payment from YourApp"
                    },
                    items = new[]
                    {
                        new
                        {
                            recipient_type = "EMAIL",
                            amount = new
                            {
                                value = amount.ToString("0.00"),
                                currency = "USD"
                            },
                            note = "Payment from YourApp",
                            receiver = recipientEmail,
                            sender_item_id = Guid.NewGuid().ToString()
                        }
                    }
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://api.sandbox.paypal.com/v1/payments/payouts", content);

                response.EnsureSuccessStatusCode();

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Handle exception
                return false;
            }
        }

    }
}
