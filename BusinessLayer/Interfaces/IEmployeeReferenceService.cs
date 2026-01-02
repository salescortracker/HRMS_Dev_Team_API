using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IEmployeeReferenceService
    {
        Task<IEnumerable<EmployeeReferenceDto>> GetAllAsync();
        Task<IEnumerable<EmployeeReferenceDto>> GetByUserIdAsync(int userId);
        Task<EmployeeReferenceDto?> GetByIdAsync(int referenceId);
        Task<int> AddAsync(EmployeeReferenceDto model);
        Task<bool> UpdateAsync(EmployeeReferenceDto model);
        Task<bool> DeleteAsync(int referenceId);
    }
}
