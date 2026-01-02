using BusinessLayer.DTOs;
namespace BusinessLayer.Interfaces
{
    public interface ICompanyNewsService
    {
        Task<int> CreateAsync(CompanyNewsDto dto);
        Task UpdateAsync(CompanyNewsDto dto);
        Task<List<CompanyNewsDto>> GetAllAsync(int companyId, int regionId);
        Task DeleteAsync(int newsId);
        Task<List<CompanyNewsDto>> GetForSuperAdminAsync(string category);
    }
}
