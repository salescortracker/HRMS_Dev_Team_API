using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IEmployeeDdlistService
    {
        Task<IEnumerable<EmployeeDdlistDto>> GetAllAsync();
        Task<EmployeeDdlistDto?> GetByIdAsync(int id);
        Task<bool> AddAsync(EmployeeDdlistDto dto);
        Task<bool> UpdateAsync(EmployeeDdlistDto dto);
        Task<bool> DeleteAsync(int id);
    }
}