
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IEmployeeFormService
    {
        Task<IEnumerable<EmployeeFormDto>> GetAllAsync();
        Task<IEnumerable<EmployeeFormDto>> GetByUserIdAsync(int userId);
        Task<EmployeeFormDto?> GetByIdAsync(int id);
        Task<int> AddAsync(EmployeeFormDto model);
        Task<bool> UpdateAsync(EmployeeFormDto model);
        Task<bool> DeleteAsync(int id);
    }
}
