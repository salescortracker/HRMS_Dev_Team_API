using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IEmployeeFamilyService
    {
        Task<IEnumerable<EmployeeFamilyDto>> GetAllAsync();
        Task<IEnumerable<EmployeeFamilyDto>> GetByUserIdAsync(int userId);
        Task<EmployeeFamilyDto?> GetByIdAsync(int familyId);
        Task<int> AddAsync(EmployeeFamilyDto model);
        Task<bool> UpdateAsync(EmployeeFamilyDto model);
        Task<bool> DeleteAsync(int familyId);

        // helper to fetch active relationships for dropdowns
        Task<IEnumerable<RelationshipDto>> GetRelationshipListAsync();
        Task<IEnumerable<DropdownGenderDto>> GetGenderListAsync();

    }
}
