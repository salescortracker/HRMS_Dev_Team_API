using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IEmployeeImmigrationService
    {
        Task<List<EmployeeImmigrationDto>> GetAllImmigrationAsync();
        Task<EmployeeImmigrationDto> GetByIdImmigrationAsync(int id);
        Task<bool> CreateImmigrationAsync(EmployeeImmigrationDto dto);
        Task<bool> UpdateImmigrationAsync(EmployeeImmigrationDto dto);
        Task<bool> DeleteImmigrationAsync(int id);

        Task<List<VisaTypeDto>> GetVisaTypesAsync();
        Task<List<WorkAuthStatusDto>> GetStatusListAsync();
    }

}

