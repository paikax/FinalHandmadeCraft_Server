using System;
using System.Threading.Tasks;
using Data.Entities.User;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Service.IServices;
using Service.Utils;

namespace Service.Service
{
    public class SendMailService : ISendMailService
    {
        
        private readonly MailSettings _mailSettings;
        private readonly ILogger<SendMailService> _logger;


        public SendMailService(IOptions<MailSettings> mailSettings, ILogger<SendMailService> logger)
        {
            _mailSettings = mailSettings.Value;
            _logger = logger;
            logger.LogInformation("Create SendMailService");
        }
        
        public async Task SendMail(MailContent mailContent)
        {
            var email = new MimeMessage();
            email.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail);
            email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
            email.To.Add(MailboxAddress.Parse(mailContent.To));
            email.Subject = mailContent.Subject;
            
            var builder = new BodyBuilder();
            builder.HtmlBody = mailContent.Body;
            email.Body = builder.ToMessageBody();
            
            // use SMTP of MailKit
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            
            try
            {
                await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(email);
            }
            catch (Exception exception)
            {
                // Send mail failed, email content will be saved to mailssave folder
                System.IO.Directory.CreateDirectory("mailssave");
                var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                await email.WriteToAsync(emailsavefile);
                
                _logger.LogInformation("Error send mail, saved at - "+ emailsavefile);
                _logger.LogError(exception.Message);
            } 
            
            await smtp.DisconnectAsync(true);
            _logger.LogInformation("Sent mail to " + mailContent.To);
        }

        public async Task SendMailAsync(string email, string subject, string htmlMessage)
        {
            await SendMail(new MailContent()
            {
                To = email,
                Subject = subject,
                Body = htmlMessage,
            });
        }
        
    }
}