using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IMaritalStatusService
    {
        Task<List<MaritalStatusDto>> GetAllAsync();
        Task<bool> CreateAsync(MaritalStatusDto dto, int userId);
        Task<bool> UpdateAsync(int id, MaritalStatusDto dto, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }
}