using Data.Entities.User;

namespace Service.IServices
{
    public interface IJwtUtils
    {
        public string GenerateToken(User user);
        public int? ValidateToken(string token);
    }
}