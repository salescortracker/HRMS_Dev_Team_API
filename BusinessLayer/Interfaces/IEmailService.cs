using BusinessLayer.DTOs;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlBody);
        Task SendWelcomeEmailAsync(User user, string password);

        // Add this
        Task SendMissedPunchEmailAsync(MissedPunchRequestDto dto);
    }
}