using System.Threading.Tasks;
using Service.Utils;

namespace Service.IServices
{
    public interface ISendMailService
    {
        Task SendMail(MailContent mailContent);
        Task SendMailAsync(string email, string subject, string htmlMessage);
    }
}