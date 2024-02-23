using System.Threading.Tasks;
using Data.Entities.User;

namespace Service.IServices
{
    public interface IPaymentSetupService
    {
        Task<bool> SetupPayPal(string userId, PayPalSetupRequest request);
        Task<string> GetPayPalEmail(string userId);
    }
}