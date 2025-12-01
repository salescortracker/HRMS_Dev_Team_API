
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IEmployeeLetterService
    {
        Task<IEnumerable<EmployeeLetterDto>> GetAllAsync();
        Task<IEnumerable<EmployeeLetterDto>> GetByUserIdAsync(int userId);
        Task<EmployeeLetterDto?> GetByIdAsync(int id);
        Task<int> AddAsync(EmployeeLetterDto model);
        Task<bool> UpdateAsync(EmployeeLetterDto model);
        Task<bool> DeleteAsync(int id);
    }
}
