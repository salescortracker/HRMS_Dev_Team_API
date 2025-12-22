using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class RaiseTicketService: IRaiseTicketService
    {
        
            private readonly HRMSContext _context;
            private readonly IConfiguration _configuration;
           
        public RaiseTicketService(HRMSContext context, IConfiguration configuration)
            {
                _context = context;
                _configuration = configuration;
                
        }

            public async Task<IEnumerable<RaiseTicket>> GetAllTicketsAsync()
            {
                return await  _context.RaiseTickets.ToListAsync();
            }

            public async Task<DataAccessLayer.DBContext.RaiseTicket?> GetTicketByIdAsync(int id)
            {
                return await _context.RaiseTickets.FindAsync(id);
            }

            public async Task<RaiseTicket> CreateTicketAsync(RaiseTicketCreateDto RaiseTicketDto)
            {
                try
                {
                    if (RaiseTicketDto == null)
                        throw new ArgumentNullException(nameof(RaiseTicketDto));

                    //// ✅ Auto-generate Employee Code if not provided
                    //string newEmployeeCode = RaiseTicketDto.EmployeeCode ?? await GenerateNextEmployeeCodeAsync();

                    //// ✅ Hash Password
                    //string hashedPassword = HashPassword(RaiseTicketDto.Password);

                    // ✅ Create RaiseTicket Entity
                    var RaiseTicket = new DataAccessLayer.DBContext.RaiseTicket
                    {
                        CompanyId = RaiseTicketDto.CompanyID,
                        RegionId = RaiseTicketDto.RegionID,
                        DepartmentId = RaiseTicketDto.DepartmentID,
                        CategoryId = RaiseTicketDto.CategoryID,
                        SubjectIssue = RaiseTicketDto.SubjectIssue,
                        Priority = RaiseTicketDto.Priority,
                        Status = "Open",
                        Description = RaiseTicketDto.Description,
                        UploadPicPath = RaiseTicketDto.UploadPicPath,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = RaiseTicketDto.CreatedBy,

                    };

                    _context.RaiseTickets.Add(RaiseTicket);
                    await _context.SaveChangesAsync();

                    //// ✅ Send Welcome Email
                    //await SendWelcomeEmailAsync(
                    //   RaiseTicket, RaiseTicketDto.Password
                    //);


                    return RaiseTicket;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        
            public async Task<RaiseTicket?> UpdateTicketAsync(int id, RaiseTicketUpdateDto updatedRaiseTicket)
           
        {
                var existingRaiseTicket = await _context.RaiseTickets.FindAsync(id);
                if (existingRaiseTicket == null) return null;

                existingRaiseTicket.SubjectIssue = updatedRaiseTicket.SubjectIssue;
                existingRaiseTicket.Description = updatedRaiseTicket.Description;
                existingRaiseTicket.Priority = updatedRaiseTicket.Priority;
                 existingRaiseTicket.CategoryId = updatedRaiseTicket.CategoryID;
                existingRaiseTicket.DepartmentId = updatedRaiseTicket.DepartmentID;
                existingRaiseTicket.UploadPicPath = updatedRaiseTicket.UploadPicPath;
                existingRaiseTicket.ModifiedAt = DateTime.UtcNow;
                existingRaiseTicket.ModifiedBy = updatedRaiseTicket.ModifiedBy;
            //
            
                // existingRaiseTicket.IsActive = updatedRaiseTicket.IsActive;

                await _context.SaveChangesAsync();
                return existingRaiseTicket;
            }

            public async Task<bool> DeleteRaiseTicketAsync(int id)
            {
                var RaiseTicket = await _context.RaiseTickets.FindAsync(id);
                if (RaiseTicket == null) return false;

                _context.RaiseTickets.Remove(RaiseTicket);
                await _context.SaveChangesAsync();
                return true;
            }

        
        
    }
}
