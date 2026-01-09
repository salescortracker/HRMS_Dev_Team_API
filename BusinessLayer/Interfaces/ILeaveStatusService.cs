using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Common;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface ILeaveStatusService
    {
        Task<ApiResponse<IEnumerable<LeaveStatusDto>>> GetAllAsync();
        Task<ApiResponse<LeaveStatusDto?>> GetByIdAsync(int id);
        Task<ApiResponse<string>> CreateAsync(CreateUpdateLeaveStatusDto dto);
        Task<ApiResponse<string>> UpdateAsync(CreateUpdateLeaveStatusDto dto);
        Task<ApiResponse<string>> DeleteAsync(int id);

    }
}
