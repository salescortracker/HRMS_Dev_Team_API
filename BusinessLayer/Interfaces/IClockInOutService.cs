using BusinessLayer.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IClockInOutService
    {
        Task<IEnumerable<ClockInOutDto>> GetAllAsync();
        Task<ClockInOutDto?> GetByIdAsync(int id);
        Task<IEnumerable<ClockInOutDto>> GetTodayByEmployeeAsync(string employeeCode, int companyId, int regionId);
        Task<ClockInOutDto> AddAsync(ClockInOutCreateDto dto, int userId);
        Task<bool> DeleteAsync(int id, int userId);
        Task<List<EarlyDepartureDto>> GetEarlyDeparturesAsync(
        int companyId,
        int regionId,
        DateOnly fromDate,
        DateOnly toDate,
        string? employeeCode);
        Task<List<LateArrivalDto>> GetLateArrivalsAsync(
       int companyId,
       int regionId,
       DateOnly fromDate,
       DateOnly toDate,
       string? employeeCode
   );
        Task<List<EmployeeDropdownDto>> GetEmployeesByCompanyRegionAsync(
           int companyId,
           int regionId
       );
    }
}
