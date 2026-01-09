using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Common;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IAttendanceStatusService
    {
        Task<ApiResponse<IEnumerable<AttendanceStatusDto>>> GetAllAsync();
        Task<ApiResponse<AttendanceStatusDto?>> GetByIdAsync(int id);
        Task<ApiResponse<string>> CreateAsync(CreateUpdateAttendanceStatusDto dto);
        Task<ApiResponse<string>> UpdateAsync(CreateUpdateAttendanceStatusDto dto);
        Task<ApiResponse<string>> DeleteAsync(int id);
    }
}
