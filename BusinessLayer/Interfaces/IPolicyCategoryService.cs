using BusinessLayer.DTOs;
using DataAccessLayer.DBContext;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IPolicyCategoryService
    {
        Task<List<PolicyCategoryDto>> GetPolicyAsync(int companyId, int regionId);
        Task<PolicyCategoryDto?> GetPolicyByIdAsync(int id);
        Task<bool> CreatePolicyAsync(PolicyCategoryDto dto);
        Task<bool> UpdatePolicyAsync(int id, PolicyCategoryDto dto);
        Task<bool> DeletePolicyAsync(int id, int userId);
        Task<bool> BulkUploadAsync(IFormFile file, int companyId, int regionId, int userId);


    }

}
