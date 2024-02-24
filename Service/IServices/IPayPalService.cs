using System.Threading.Tasks;
using Data.Entities.User;
using Microsoft.AspNetCore.Mvc;

namespace Service.IServices
{
    public interface IPayPalService
    {
        public Task<string> CreateOrder(decimal amount);
        public Task<bool> CaptureOrder(string orderId);
        public Task<bool> SendPayment(string recipientEmail, decimal amount);
    }
}