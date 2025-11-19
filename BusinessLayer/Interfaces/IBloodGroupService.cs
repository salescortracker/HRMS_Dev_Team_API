using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IBloodGroupService
    {
        Task<IEnumerable<BloodGroupDto>> GetAllAsync();
        Task<BloodGroupDto> GetByIdAsync(int id);
        Task<string> CreateAsync(BloodGroupDto dto, int userId);
        Task<string> UpdateAsync(BloodGroupDto dto, int userId);
        Task<string> DeleteAsync(int id, int userId);
    }
}
