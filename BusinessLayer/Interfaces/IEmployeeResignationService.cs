using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IEmployeeResignationService
    {
        // Required by old API
        Task<IEnumerable<EmployeeResignationDto>> GetAllResignationsAsync();
        Task<IEnumerable<EmployeeResignationDto>> SearchResignationsAsync(object filter);
        Task<IEnumerable<DataAccessLayer.DBContext.EmployeeResignation>> AddMultipleResignationsAsync(List<EmployeeResignationDto> dtos);

        // Your new filtered methods
        Task<IEnumerable<EmployeeResignationDto>> GetResignationsByCompanyRegionAsync(int companyId, int regionId);
        Task<EmployeeResignationDto?> GetResignationByIdFilteredAsync(int id, int companyId, int regionId);

        // CRUD
        Task<EmployeeResignationDto?> GetResignationByIdAsync(int id);
        Task<EmployeeResignationDto> AddResignationAsync(EmployeeResignationDto dto);
        Task<EmployeeResignationDto> UpdateResignationAsync(int id, EmployeeResignationDto dto);
        Task<bool> DeleteResignationAsync(int id, int companyId, int regionId);
    }
}
