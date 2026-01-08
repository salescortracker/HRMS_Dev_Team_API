using BusinessLayer.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IExpenseCategoryService
    {
        Task<List<ExpenseCategoryDto>> GetAllExpencesAsync(int companyId, int regionId);
        Task CreateExpencesAsync(ExpenseCategoryDto dto, int userId);
        Task UpdateExpencesAsync(ExpenseCategoryDto dto, int userId);
        Task DeleteExpencesAsync(int id);
    }
}
