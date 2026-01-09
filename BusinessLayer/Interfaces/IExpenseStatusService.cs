using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Common;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IExpenseStatusService
    {
        Task<ApiResponse<IEnumerable<ExpenseStatusDto>>> GetAllAsync();

        Task<ApiResponse<ExpenseStatusDto?>> GetByIdAsync(int id);

        Task<ApiResponse<string>> SaveAsync(CreateUpdateExpenseStatusDto dto); // CREATE + UPDATE (POST)

        Task<ApiResponse<string>> DeleteAsync(int id);

    }
}
