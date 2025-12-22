using BusinessLayer.DTOs;
using DataAccessLayer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IDigitalService
    {
        Task<DigitalCardDto> GetDigitalCardAsync(int userId);
        Task<EmployeeImageRequestDto> employeeimage(int userId);

    }
}
