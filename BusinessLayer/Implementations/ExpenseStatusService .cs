using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;

namespace BusinessLayer.Implementations
{
    public class ExpenseStatusService : IExpenseStatusService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExpenseStatusService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // -----------------------------
        // GET ALL (NO COMPANY / REGION)
        // -----------------------------
        public async Task<ApiResponse<IEnumerable<ExpenseStatusDto>>> GetAllAsync()
        {
            try
            {
                var list = (await _unitOfWork.Repository<ExpenseStatus>()
                    .FindAsync(x => !x.IsDeleted))
                    .OrderByDescending(x => x.ExpenseStatusId)
                    .Select(x => new ExpenseStatusDto
                    {
                        ExpenseStatusID = x.ExpenseStatusId,
                        CompanyID = x.CompanyId,
                        RegionID = x.RegionId,
                        ExpenseStatusName = x.ExpenseStatusName ?? string.Empty,
                        IsActive = x.IsActive
                    });

                return new ApiResponse<IEnumerable<ExpenseStatusDto>>(
                    list,
                    "Expense Status retrieved successfully."
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<ExpenseStatusDto>>(
                    null!,
                    $"Failed to retrieve Expense Status. {ex.Message}",
                    false
                );
            }
        }

        // -----------------------------
        // GET BY ID
        // -----------------------------
        public async Task<ApiResponse<ExpenseStatusDto?>> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.Repository<ExpenseStatus>().GetByIdAsync(id);

            if (entity == null || entity.IsDeleted)
                return new ApiResponse<ExpenseStatusDto?>(null, "Expense Status not found.", false);

            var dto = new ExpenseStatusDto
            {
                ExpenseStatusID = entity.ExpenseStatusId,
                CompanyID = entity.CompanyId,
                RegionID = entity.RegionId,
                ExpenseStatusName = entity.ExpenseStatusName ?? string.Empty,
                IsActive = entity.IsActive
            };

            return new ApiResponse<ExpenseStatusDto?>(dto, "Expense Status retrieved successfully.");
        }

        // -----------------------------
        // CREATE + UPDATE (POST)
        // -----------------------------
        public async Task<ApiResponse<string>> SaveAsync(CreateUpdateExpenseStatusDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.ExpenseStatusName))
                    return new ApiResponse<string>(null!, "Expense Status Name is required.", false);

                // DUPLICATE CHECK
                var duplicate = (await _unitOfWork.Repository<ExpenseStatus>().FindAsync(x =>
                    !x.IsDeleted &&
                    x.CompanyId == dto.CompanyID &&
                    x.RegionId == dto.RegionID &&
                    x.ExpenseStatusName != null &&
                    x.ExpenseStatusName.ToLower() == dto.ExpenseStatusName.ToLower() &&
                    x.ExpenseStatusId != dto.ExpenseStatusID))
                    .Any();

                if (duplicate)
                    return new ApiResponse<string>(null!, "Duplicate Expense Status exists.", false);

                // CREATE
                if (dto.ExpenseStatusID == 0)
                {
                    var entity = new ExpenseStatus
                    {
                        CompanyId = dto.CompanyID,
                        RegionId = dto.RegionID,
                        ExpenseStatusName = dto.ExpenseStatusName,
                        IsActive = dto.IsActive,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.Repository<ExpenseStatus>().AddAsync(entity);
                    await _unitOfWork.CompleteAsync();

                    return new ApiResponse<string>("Expense Status created successfully.");
                }
                // UPDATE
                else
                {
                    var entity = await _unitOfWork.Repository<ExpenseStatus>()
                        .GetByIdAsync(dto.ExpenseStatusID);

                    if (entity == null || entity.IsDeleted)
                        return new ApiResponse<string>(null!, "Expense Status not found.", false);

                    entity.ExpenseStatusName = dto.ExpenseStatusName;
                    entity.IsActive = dto.IsActive;
                    entity.ModifiedAt = DateTime.UtcNow;

                    _unitOfWork.Repository<ExpenseStatus>().Update(entity);
                    await _unitOfWork.CompleteAsync();

                    return new ApiResponse<string>("Expense Status updated successfully.");
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(
                    null!,
                    $"Save failed. {ex.Message}",
                    false
                );
            }
        }

        // -----------------------------
        // DELETE (HARD DELETE)
        // -----------------------------
        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Repository<ExpenseStatus>().GetByIdAsync(id);

            if (entity == null)
                return new ApiResponse<string>(null!, "Expense Status not found.", false);

            _unitOfWork.Repository<ExpenseStatus>().Delete(entity);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<string>("Expense Status deleted successfully.");
        }
    }
}
