using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IBloodGroupService
    {
        Task<IEnumerable<BloodGroupDto>> GetAllAsync();
        Task<BloodGroupDto?> GetByIdAsync(int id); // ✅ nullable
        Task<string> CreateAsync(BloodGroupDto dto);
        Task<string> UpdateAsync(BloodGroupDto dto);
        Task<string> DeleteAsync(int id);



    }
}
