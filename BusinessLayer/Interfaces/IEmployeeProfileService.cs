using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IEmployeeProfileService
    {
        Task<EmployeeProfileDto?> GetEmployeeProfileAsync(int userId);
        Task<int> SaveEmployeeImageAsync(EmployeeImageRequestDto dto);
    }
}
