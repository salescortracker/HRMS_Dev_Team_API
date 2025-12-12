using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly HRMSContext _context;

        public EmailService(IConfiguration configuration, HRMSContext context)
        {
            _configuration = configuration;
            _context = context;
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

        // Send Welcome Email (existing)
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

        // ===============================
        // Send Missed Punch Request Email
        // ===============================
        public async Task SendMissedPunchEmailAsync(MissedPunchRequestDto dto)
        {
            if (dto.ManagerID == 0) return;

            var manager = await _context.Users.FirstOrDefaultAsync(u => u.UserId == dto.ManagerID);
            if (manager == null) return;

            string subject = $"Missed Punch Request from Employee {dto.EmployeeID}";
            string body = $@"
        <p>Employee ID: {dto.EmployeeID}</p>
        <p>Missed Date: {dto.MissedDate:yyyy-MM-dd}</p>
        <p>Missed Type: {dto.MissedType}</p>
        <p>Clock In: {(dto.CorrectClockIn.HasValue ? dto.CorrectClockIn.Value.ToString("HH:mm") : "-")}</p>
        <p>Clock Out: {(dto.CorrectClockOut.HasValue ? dto.CorrectClockOut.Value.ToString("HH:mm") : "-")}</p>
        <p>Reason: {dto.Reason}</p>
        <br/>
        <p>
            <a href='https://frontend/manager/missedpunch/approve?requestId={dto.MissedPunchRequestID}'>Approve</a> | 
            <a href='https://frontend/manager/missedpunch/reject?requestId={dto.MissedPunchRequestID}'>Reject</a>
        </p>
    ";

            await SendEmailAsync(manager.Email, subject, body);
        }

    }
}