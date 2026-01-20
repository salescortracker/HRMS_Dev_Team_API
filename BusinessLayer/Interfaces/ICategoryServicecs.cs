
using BusinessLayer.DTOs;
namespace BusinessLayer.Interfaces
{
    public interface ICategoryServicecs
    {

        Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync();
        Task<IEnumerable<CompanyPolicyDto>> GetAllAsync(int companyId, int regionId);
        Task<CompanyPolicyDto?> GetByIdAsync(int id);
        Task<CompanyPolicyDto> AddAsync(CompanyPolicyDto dto);
        Task<CompanyPolicyDto> UpdateAsync(int id, CompanyPolicyDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<CompanyPolicyDto>> GetAllPoliciesAsync();
    }
}
