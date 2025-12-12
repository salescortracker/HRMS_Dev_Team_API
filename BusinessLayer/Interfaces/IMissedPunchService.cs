using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IMissedPunchService
    {
        Task<List<MissedPunchRequestDto>> GetMyRequestsAsync(int employeeId);
        Task<List<MissedPunchRequestDto>> GetPendingRequestsForManagerAsync(int managerId);
        Task<MissedPunchRequestDto> SubmitRequestAsync(MissedPunchRequestDto dto);
        Task<bool> TakeActionAsync(MissedPunchActionDto actionDto);
        Task<List<MissedTypeDto>> GetActiveMissedTypesAsync();
    }
}