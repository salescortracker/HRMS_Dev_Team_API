using BusinessLayer.DTOs;


namespace BusinessLayer.Interfaces
{
    public interface IEmployeePersonalService
    {
        Task<IEnumerable<EmployeePersonalDetailDto>> GetAllAsync();
        Task<IEnumerable<EmployeePersonalDetailDto>> GetByUserIdAsync(int userId);
        Task<EmployeePersonalDetailDto?> GetByIdAsync(int id);
        Task<int> AddAsync(EmployeePersonalDetailDto model);
        Task<bool> UpdateAsync(EmployeePersonalDetailDto model);
        Task<bool> DeleteAsync(int id);
    }
}
