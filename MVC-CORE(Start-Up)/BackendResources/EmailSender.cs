using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MVC_CORE_Start_Up_.BackendResources
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var host = _configuration["Smtp:Host"];
            var port = int.Parse(_configuration["Smtp:Port"]);
            var fromAddress = _configuration["Smtp:FromAddress"];
            var password = _configuration["Smtp:Password"];

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(fromAddress, password),
                EnableSsl = true // Gmail requires SSL/TLS
            };

            var mail = new MailMessage(fromAddress, email, subject, htmlMessage)
            {
                IsBodyHtml = true
            };

            try
            {
                await client.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SMTP ERROR: {ex.Message}");
                throw;
            }
        }
    }
}
