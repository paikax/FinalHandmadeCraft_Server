using System.Threading.Tasks;
using Data.Entities.User;

namespace Service.IServices
{
    public interface IPayPalService
    {
        public Task<string> CreateOrder(decimal amount);
        public Task<bool> CaptureOrder(string orderId);
    }
}