using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
   
    public interface ILateArrivalService
    {
        Task<List<LateArrivalDto>> GetLateArrivalsAsync(
        int companyId,
        int regionId,
        DateOnly fromDate,
        DateOnly toDate,
        string? employeeCode
    );
    }

}
