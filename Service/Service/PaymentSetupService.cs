using System.Threading.Tasks;
using Data.Context;
using Data.Entities.User;
using Service.IServices;

namespace Service.Service
{
    public class PaymentSetupService : IPaymentSetupService
    {
        private readonly AppDbContext _db;

        public PaymentSetupService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> SetupPayPal(string userId, PayPalSetupRequest request)
        {
            var user = await _db.Users.FindAsync(userId);

            if (user == null)
            {
                // Handle user not found
                return false;
            }

            // Store PayPal client ID and secret
            user.PayPalClientId = request.ClientId;
            user.PayPalClientSecret = request.ClientSecret;
            user.IsPayPalLinked = true;

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<string> GetPayPalEmail(string userId)
        {
            var user = await _db.Users.FindAsync(userId);

            if (user == null || string.IsNullOrEmpty(user.PayPalEmail))
            {
                return null;
            }

            return user.PayPalEmail;
        }
    }
}