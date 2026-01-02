using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IEmployeeEducationService
    {
        Task<IEnumerable<EmployeeEducationDto>> GetByUserIdAsync(int userId);
        Task<EmployeeEducationDto?> GetByIdAsync(int educationId);
        Task<int> AddAsync(EmployeeEducationDto model);
        Task<bool> UpdateAsync(EmployeeEducationDto model);
        Task<bool> DeleteAsync(int educationId);
        Task<IEnumerable<EmployeeEducationDto>> GetAllAsync();
        Task<IEnumerable<ModeOfStudyDto>> GetModeOfStudyListAsync();
    }
}
