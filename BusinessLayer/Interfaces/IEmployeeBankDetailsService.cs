using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IEmployeeBankDetailsService
    {
        Task<IEnumerable<EmployeeBankDetailsDto>> GetAllAsync();
        Task<EmployeeBankDetailsDto?> GetByIdAsync(int id);
        Task<bool> AddAsync(EmployeeBankDetailsDto dto);
        Task<bool> UpdateAsync(EmployeeBankDetailsDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
