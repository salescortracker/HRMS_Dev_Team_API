using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface ILeaveTypeService
    {
        Task<List<LeaveTypeDto>> GetLeaveTypesAsync(); 
        Task<bool> CreateLeaveTypeAsync(LeaveTypeDto dto);
        Task<bool> UpdateLeaveTypeAsync(LeaveTypeDto dto);
        Task<bool> DeleteLeaveTypeAsync(int id);
    }
}
