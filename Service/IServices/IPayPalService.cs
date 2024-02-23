using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IPayPalService
    {
        public Task<string> CreateOrder(decimal amount);
        public Task<bool> CaptureOrder(string orderId);
    }
}