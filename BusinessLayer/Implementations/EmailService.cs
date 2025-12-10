using BusinessLayer.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.DBContext;

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
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Smtp:FromEmail"], "Cortracker HRMS"),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            using var smtpClient = CreateSmtpClient();
            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendWelcomeEmailAsync(User user, string password)
        {
            string subject = "Welcome to HRMS – Your Login Details";
            string body = $@"
                <p>Hello {user.FullName},</p>
                <p>Your account has been created.</p>
                <p>Email: {user.Email}</p>
                <p>Password: {password}</p>";

            await SendEmailAsync(user.Email, subject, body);
        }
    }
}