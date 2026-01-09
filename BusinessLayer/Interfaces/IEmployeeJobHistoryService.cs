using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IEmployeeJobHistoryService
    {
        Task<IEnumerable<EmployeeJobHistoryDto>> GetAllAsync();
        Task<IEnumerable<EmployeeJobHistoryDto>> GetByUserIdAsync(int userId);
        Task<EmployeeJobHistoryDto?> GetByIdAsync(int id);
        Task<int> AddAsync(EmployeeJobHistoryDto model);
        Task<bool> UpdateAsync(EmployeeJobHistoryDto model);
        Task<bool> DeleteAsync(int id);

    }
}
