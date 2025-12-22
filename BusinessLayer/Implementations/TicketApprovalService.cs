using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class TicketApprovalService : ITicketApprovalService
    {
        private readonly HRMSContext _context;
        private readonly IConfiguration _configuration;
        
        public TicketApprovalService(HRMSContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }
        public async Task<IEnumerable<RaiseTicketApproval>> GetApprovalsAsync()
        {
            return await _context.RaiseTicketApprovals.AsNoTracking().ToListAsync();
        }

        public async Task UpdateApprovalAsync(int id, RaiseTicketApprovalUpdateDto dto)
        {
            var approval = await _context.RaiseTicketApprovals.FindAsync(id);
            if (approval == null) throw new KeyNotFoundException();


            approval.ManagerStatus = dto.ManagerStatus;
            approval.ManagerComments = dto.ManagerComments;
            approval.ManagerId = dto.ManagerId;
            approval.ModifiedBy = dto.ModifiedBy;
            approval.ModifiedAt = dto.ModifiedAt ?? DateTime.UtcNow;

            var ticket = await _context.RaiseTickets.FindAsync(dto.RaiseTicketId);
            if (ticket != null && !string.IsNullOrWhiteSpace(dto.TicketStatus))
            {
                ticket.Status = dto.TicketStatus;
                ticket.ModifiedBy = dto.ModifiedBy;
                ticket.ModifiedAt = DateTime.UtcNow;
            }



            await _context.SaveChangesAsync();
        }

        public async Task BulkApproveAsync(BulkTicketApprovalDto dto)
        {
            var approvals = await _context.RaiseTicketApprovals
                .Where(a => dto.RaiseTicketApprovalIds.Contains(a.RaiseTicketApprovalId))
                .ToListAsync();

            foreach (var a in approvals)
            {
                a.ManagerStatus = 1; // Approved
                if (!string.IsNullOrWhiteSpace(dto.ManagerComments))
                    a.ManagerComments = dto.ManagerComments;
                a.ManagerId = dto.ManagerId;
                a.ModifiedBy = dto.ActionBy;
                a.ModifiedAt = DateTime.UtcNow;

                var ticket = await _context.RaiseTickets.FindAsync(a.RaiseTicketId);
                if (ticket != null && !string.IsNullOrWhiteSpace(dto.TicketStatus))
                {
                    ticket.Status = dto.TicketStatus;
                    ticket.ModifiedBy = dto.ActionBy;
                    ticket.ModifiedAt = DateTime.UtcNow;
                }

            }

            await _context.SaveChangesAsync();
        }

        public async Task BulkRejectAsync(BulkTicketApprovalDto dto)
        {
            var approvals = await _context.RaiseTicketApprovals
                .Where(a => dto.RaiseTicketApprovalIds.Contains(a.RaiseTicketApprovalId))
                .ToListAsync();

            foreach (var a in approvals)
            {
                a.ManagerStatus = 2; // Rejected
                if (!string.IsNullOrWhiteSpace(dto.ManagerComments))
                    a.ManagerComments = dto.ManagerComments;
                a.ManagerId = dto.ManagerId;
                a.ModifiedBy = dto.ActionBy;
                a.ModifiedAt = DateTime.UtcNow;

                var ticket = await _context.RaiseTickets.FindAsync(a.RaiseTicketId);
                if (ticket != null && !string.IsNullOrWhiteSpace(dto.TicketStatus))
                {
                    ticket.Status = dto.TicketStatus;
                    ticket.ModifiedBy = dto.ActionBy;
                    ticket.ModifiedAt = DateTime.UtcNow;
                }

            }

            await _context.SaveChangesAsync();
        }

        
    }
}
