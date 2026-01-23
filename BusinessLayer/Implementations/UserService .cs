using BCrypt.Net;
using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;


namespace BusinessLayer.Implementations
{
    public class UserService : IUserService
    {
        private readonly DataAccessLayer.DBContext.HRMSContext _context;
        private readonly IConfiguration _configuration;

        public UserService(DataAccessLayer.DBContext.HRMSContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IEnumerable<DataAccessLayer.DBContext.User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<DataAccessLayer.DBContext.User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> CreateUserAsync(UserCreateDto userDto)
        {
            try
            {
                if (userDto == null)
                    throw new ArgumentNullException(nameof(userDto));

                // ✅ Auto-generate Employee Code if not provided
                string newEmployeeCode = userDto.EmployeeCode ?? await GenerateNextEmployeeCodeAsync();



                // ✅ Hash Password
                string hashedPassword = HashPassword(userDto.Password);

                // ✅ Create User Entity
                var user = new DataAccessLayer.DBContext.User
                {
                    CompanyId = userDto.CompanyID,
                    RegionId = userDto.RegionID,
                    EmployeeCode = newEmployeeCode,
                    FullName = userDto.FullName,
                    Email = userDto.Email,
                    PasswordHash = hashedPassword,
                    RoleId = userDto.RoleId,
                    Status = "Active",
                    CreatedDate = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var loginStatus = new UserLoginStatus
                {
                    UserId = user.UserId,
                    MustChangePassword = true

                };

                _context.UserLoginStatuses.Add(loginStatus);
                await _context.SaveChangesAsync();

                // ✅ Send Welcome Email
                await SendWelcomeEmailAsync(
                   user,userDto.Password
                );


                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    
        public async Task<object?> VerifyLoginAsync(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    throw new ArgumentException("Username or password cannot be empty.");


                Console.WriteLine("Verifying login for user: " + username);
                Console.WriteLine("Password provided: " + password);
                //1. Get user by email
                var user = await _context.Users
                    .Include(u => u.UserLoginStatus)
                    .FirstOrDefaultAsync(u => u.Email == username);
                Console.WriteLine("User: " + user);

                if (user == null)
                    return null;

                // 2. Detect Hashing Scheme
              
                string incomingHash = HashPassword(password);
                Console.WriteLine("Incoming Hash: " + incomingHash);

                // 3.Verify password
                if (user.PasswordHash != incomingHash)
                  return null;

                // 4. Compute mustChangePassword OUTSIDE the LINQ query
                bool mustChangePassword = user.UserLoginStatus?.MustChangePassword ?? false;



                // 5. Fetch joined data
                var userData = await (from u in _context.Users
                                      join r in _context.RoleMasters on u.RoleId equals r.RoleId
                                      join reg in _context.Regions on u.RegionId equals reg.RegionId
                                      join c in _context.Companies on u.CompanyId equals c.CompanyId
                                      where u.Email== username && u.PasswordHash == incomingHash
                                      select new
                                      {
                                          u.UserId,
                                          u.Email,
                                          u.FullName,
                                          RoleName = r.RoleName,
                                          RegionName = reg.RegionName,
                                          CompanyName = c.CompanyName,
                                          roleId=u.RoleId,
                                          companyId=u.CompanyId,
                                          regionId=u.RegionId,
                                          mustChangePassword = mustChangePassword                                      })
                                     .FirstOrDefaultAsync();

               

                return userData;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Input error: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying login: {ex.Message}");
                return null;
            }
        }

     


        public async Task<ApiResponse<bool>> ChangePasswordAsync(PasswordChangeDto dto)
        {
            var user = await _context.Users
                .Include(u => u.UserLoginStatus)
                .FirstOrDefaultAsync(u => u.UserId == dto.UserId);

            if (user == null)
                return new ApiResponse<bool>(false, "User not found", false);

            // Old password check only if NOT first login
            if (!user.UserLoginStatus.MustChangePassword)
            {
                string oldHash = HashPassword(dto.OldPassword);
                if (user.PasswordHash != oldHash)
                    return new ApiResponse<bool>(false, "Old password is incorrect", false);
            }

            user.PasswordHash = HashPassword(dto.NewPassword);
            user.UserLoginStatus.MustChangePassword = false;

            await _context.SaveChangesAsync();

            return new ApiResponse<bool>(true, "Password updated successfully", true);
        }



        // 🔹 Generate Employee Code (Emp0001, Emp0002, etc.)
        private async Task<string> GenerateNextEmployeeCodeAsync()
        {
            var lastUser = await _context.Users
                .OrderByDescending(u => u.UserId)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastUser != null && !string.IsNullOrEmpty(lastUser.EmployeeCode))
            {
                string numberPart = new string(lastUser.EmployeeCode.SkipWhile(c => !char.IsDigit(c)).ToArray());
                if (int.TryParse(numberPart, out int lastNumber))
                    nextNumber = lastNumber + 1;
            }

            return $"EMP{nextNumber:D4}";
        }

        // 🔹 Simple SHA256 password hashing
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        
//
        public async Task<DataAccessLayer.DBContext.User?> UpdateUserAsync(int id, DataAccessLayer.DBContext.User updatedUser)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return null;

            existingUser.FullName = updatedUser.FullName;
            existingUser.Email = updatedUser.Email;
            existingUser.RoleId = updatedUser.RoleId;
           // existingUser.IsActive = updatedUser.IsActive;

            await _context.SaveChangesAsync();
            return existingUser;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task SendWelcomeEmailAsync(DataAccessLayer.DBContext.User user,string password)
        {
            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Host = _configuration["Smtp:Host"];
                    smtpClient.Port = int.Parse(_configuration["Smtp:Port"]);
                    smtpClient.EnableSsl = true;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(
                        _configuration["Smtp:User"],
                        _configuration["Smtp:Password"]
                    );

                    string logoUrl = "http://mock-hr.cortracker360.com/assets/images/cor-logo.png"; // Replace with your actual logo

                    string subject = "Welcome to HRMS – Your Login Details";

                    string body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Welcome to HRMS</title>
</head>
<body style='margin:0;padding:0;background-color:#f4f6f8;font-family:Segoe UI,Roboto,Helvetica,Arial,sans-serif;color:#333;'>
    <table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='background-color:#f4f6f8;padding:40px 0;'>
        <tr>
            <td align='center'>
                <table role='presentation' cellpadding='0' cellspacing='0' width='600' style='background-color:#ffffff;border-radius:10px;overflow:hidden;box-shadow:0 2px 10px rgba(0,0,0,0.08);'>
                    <tr>
                        <td style='background-color:#004aad;padding:20px;text-align:center;'>
                            <img src='{logoUrl}' alt='Cortracker HRMS' style='height:60px;'/>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding:40px 30px;'>
                            <h2 style='color:#004aad;margin-bottom:10px;'>Welcome to Cortracker HRMS, {user.FullName}!</h2>
                            <p style='font-size:16px;line-height:1.6;margin:20px 0;'>
                                We’re excited to have you onboard! Your HRMS account has been successfully created. Please find your login details below.
                            </p>

                            <table cellpadding='6' cellspacing='0' style='width:100%;margin:20px 0;border-collapse:collapse;'>
                                <tr>
                                    <td style='font-weight:bold;color:#004aad;width:120px;'>Login URL:</td>
                                    <td><a href='http://dev-hr.cortracker360.com' style='color:#004aad;text-decoration:none;'>https://ha.cortracker360.com</a></td>
                                </tr>
                                <tr>
                                    <td style='font-weight:bold;color:#004aad;'>Username:</td>
                                    <td>{user.Email}</td>
                                </tr>
                                <tr>
                                    <td style='font-weight:bold;color:#004aad;'>Password:</td>
                                    <td>{password}</td>
                                </tr>
                            </table>

                            <p style='font-size:15px;line-height:1.6;'>
                                For your security, please update your password after your first login.
                            </p>

                            <div style='margin-top:30px;text-align:center;'>
                                <a href='http://dev-hr.cortracker360.com' 
                                   style='background-color:#004aad;color:#fff;padding:12px 24px;border-radius:6px;text-decoration:none;font-weight:600;'>
                                   Go to HRMS Portal
                                </a>
                            </div>

                            <p style='font-size:14px;color:#888;margin-top:30px;'>
                                Regards,<br/>
                                <strong>HR Team</strong><br/>
                                Cortracker HRMS
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td style='background-color:#f0f2f5;text-align:center;padding:15px;font-size:12px;color:#888;'>
                            © {DateTime.UtcNow.Year} Cortracker HRMS. All rights reserved.
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";


                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_configuration["Smtp:FromEmail"], "Cortracker HRMS"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(user.Email);

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP Error: {smtpEx.StatusCode} - {smtpEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
            }
        }

        
    }
}

