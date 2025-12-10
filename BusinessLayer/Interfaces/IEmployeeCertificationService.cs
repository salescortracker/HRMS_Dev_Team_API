using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IEmployeeCertificationService
    {
        Task<IEnumerable<EmployeeCertificationDto>> GetAllAsync();
        Task<IEnumerable<EmployeeCertificationDto>> GetByUserIdAsync(int userId);
        Task<EmployeeCertificationDto?> GetByIdAsync(int certificationId);
        Task<int> AddAsync(EmployeeCertificationDto model);
        Task<bool> UpdateAsync(EmployeeCertificationDto model);
        Task<bool> DeleteAsync(int certificationId);

        Task<IEnumerable<CertificationTypeDto>> GetCertificationTypesAsync();
    }
}
