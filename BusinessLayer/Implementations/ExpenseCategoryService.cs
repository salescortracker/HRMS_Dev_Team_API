using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class ExpenseCategoryService : IExpenseCategoryService
    {
        private readonly HRMSContext _context;

        public ExpenseCategoryService(HRMSContext context)
        {
            _context = context;
        }

        public async Task<List<ExpenseCategoryDto>> GetAllExpencesAsync(int companyId, int regionId)
        {
            return await _context.ExpenseCategories 
                .Where(x => x.CompanyId == companyId && x.RegionId == regionId)
                .OrderBy(x => x.SortOrder)
                .Select(x => new ExpenseCategoryDto
                {
                    ExpenseCategoryID = x.ExpenseCategoryId,
                    ExpenseCategoryName = x.ExpenseCategoryName,
                    IsActive = x.IsActive,
                    CompanyID = x.CompanyId,
                    RegionID = x.RegionId
                })
                .ToListAsync();
        }

        public async Task CreateExpencesAsync(ExpenseCategoryDto dto, int userId)
        {
            if (dto.ExpenseCategoryName.Any(char.IsDigit))
                throw new Exception("Only letters allowed");

            var entity = new ExpenseCategory
            {

                ExpenseCategoryName = dto.ExpenseCategoryName,
                IsActive = dto.IsActive,
                CompanyId = dto.CompanyID,
                RegionId = dto.RegionID,
                CreatedBy = userId,
                CreatedDate = DateTime.Now
            };

            _context.ExpenseCategories.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateExpencesAsync(ExpenseCategoryDto dto, int userId)
        {
            var entity = await _context.ExpenseCategories
                .FirstOrDefaultAsync(x => x.ExpenseCategoryId == dto.ExpenseCategoryID);

            if (entity == null)
                throw new Exception("Expense Category not found");

            entity.ExpenseCategoryName = dto.ExpenseCategoryName;
            entity.IsActive = dto.IsActive;
            entity.UpdatedBy = userId;
            entity.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteExpencesAsync(int id)
        {
            var entity = await _context.ExpenseCategories
                .FirstOrDefaultAsync(x => x.ExpenseCategoryId == id);

            if (entity == null)
                throw new Exception("Expense Category not found");

            _context.ExpenseCategories.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
