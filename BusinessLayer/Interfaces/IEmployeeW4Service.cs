using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IEmployeeW4Service
    {
        Task<List<EmployeeW4Dto>> GetAllAsync();
        Task<EmployeeW4Dto?> GetByIdAsync(int id);
        Task<bool> AddAsync(EmployeeW4Dto dto);
        Task<bool> UpdateAsync(EmployeeW4Dto dto);
        Task<bool> DeleteAsync(int id);
    }
}