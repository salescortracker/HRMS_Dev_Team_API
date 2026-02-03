using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IEmployeeEmergencyContactService
    {
        Task<IEnumerable<EmployeeEmergencyContactDto>> GetAllAsync();
        Task<IEnumerable<EmployeeEmergencyContactDto>> GetByUserIdAsync(int userId);
        Task<EmployeeEmergencyContactDto?> GetByIdAsync(int emergencyContactId);
        Task<int> AddAsync(EmployeeEmergencyContactDto model);
        Task<bool> UpdateAsync(EmployeeEmergencyContactDto model);
        Task<bool> DeleteAsync(int emergencyContactId);

        // helper to fetch active relationships for dropdowns (same as family)
        Task<IEnumerable<RelationshipDto>> GetRelationshipListAsync();

    }
}
