using Microsoft.Extensions.Options;
using SimpleCMSWeb.Models.EmailView;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SimpleCMSWeb.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpClient = new SmtpClient(_emailSettings.Host)
            {
                Port = _emailSettings.Port,
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = true,
            };
                
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.Username),
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
            };

            mailMessage.To.Add(to);
            smtpClient.Send(mailMessage);
            //await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
