
using DataAccessLayer.Models;

namespace BusinessLayer.Interfaces
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(User user, string password);
        Task SendEmailAsync(string to, string subject, string htmlBody);
    }
}
