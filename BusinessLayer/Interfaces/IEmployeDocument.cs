
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IEmployeDocument
    {
        Task<IEnumerable<DocumentTypeDto>> GetActiveDocumentTypesAsync();
        Task<IEnumerable<EmployeeDocumentDto>> GetAllAsync();
        Task<IEnumerable<EmployeeDocumentDto>> GetByUserIdAsync(int userId);
        Task<EmployeeDocumentDto?> GetByIdAsync(int id);
        Task<int> AddAsync(EmployeeDocumentDto model);
        Task<bool> UpdateAsync(EmployeeDocumentDto model);
        Task<bool> DeleteAsync(int id);
    }
}
