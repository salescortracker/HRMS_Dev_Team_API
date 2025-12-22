using BusinessLayer.DTOs;
using DataAccessLayer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface ITicketApprovalService
    {
        Task<IEnumerable<RaiseTicketApproval>> GetApprovalsAsync();
        Task UpdateApprovalAsync(int id, RaiseTicketApprovalUpdateDto dto);
        Task BulkApproveAsync(BulkTicketApprovalDto dto);
        Task BulkRejectAsync(BulkTicketApprovalDto dto);
    }
}
