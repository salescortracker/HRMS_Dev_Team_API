
using System.Net.Mail;
using System.Net;
using BusinessLayer.Interfaces;
using Microsoft.Extensions.Configuration;
using DataAccessLayer.Models;

namespace BusinessLayer.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SmtpClient CreateSmtpClient()
        {
            return new SmtpClient
            {
                Host = _configuration["Smtp:Host"],
                Port = int.Parse(_configuration["Smtp:Port"]),
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    _configuration["Smtp:User"],
                    _configuration["Smtp:Password"]
                )
            };
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            using var smtpClient = CreateSmtpClient();

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Smtp:FromEmail"], "Cortracker HRMS"),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);
            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendWelcomeEmailAsync(User user, string password)
        {
            string logoUrl = _configuration["AppSettings:LogoUrl"];
            string portalUrl = _configuration["AppSettings:PortalUrl"];

            string subject = "Welcome to HRMS – Your Login Details";

            string body = $@"
        <!DOCTYPE html>
        <html>
        <body style='font-family:Segoe UI'>
            <h2>Welcome {user.FullName}</h2>
            <p>Your HRMS account has been created.</p>
            <p><b>Login URL:</b> <a href='{portalUrl}'>{portalUrl}</a></p>
            <p><b>Username:</b> {user.Email}</p>
            <p><b>Password:</b> {password}</p>
        </body>
        </html>";

            await SendEmailAsync(user.Email, subject, body);
        }
    }
}
