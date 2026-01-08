using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class PolicyCategoryService : IPolicyCategoryService
    {
        private readonly HRMSContext _context;

        public PolicyCategoryService(HRMSContext context)
        {
            _context = context;
        }

        // GET ALL (Filtered)
        public async Task<List<PolicyCategoryDto>> GetPolicyAsync(int companyId, int regionId)
        {
            return await _context.PolicyCategories
                .Where(x =>
                    (companyId == 0 || x.CompanyId == companyId) &&
                    (regionId == 0 || x.RegionId == regionId) &&
                    (x.IsDeleted == false || x.IsDeleted == null))
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => MapToDto(x))
                .ToListAsync();
        }

        // GET BY ID
        public async Task<PolicyCategoryDto?> GetPolicyByIdAsync(int id)
        {
            var entity = await _context.PolicyCategories
                .FirstOrDefaultAsync(x =>
                    x.PolicyCategoryId == id &&
                    (x.IsDeleted == false || x.IsDeleted == null));

            return entity == null ? null : MapToDto(entity);
        }

        // CREATE
        public async Task<bool> CreatePolicyAsync(PolicyCategoryDto dto)
        {
            try
            {
                var entity = new PolicyCategory
                {
                    PolicyCategoryName = dto.PolicyCategoryName,
                    Description = dto.Description,
                    IsActive = dto.IsActive,
                    CompanyId = dto.CompanyId,
                    RegionId = dto.RegionId,
                    CreatedBy = dto.UserId,
                    CreatedAt = DateTime.Now
                };

                _context.PolicyCategories.Add(entity);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                throw; // controller will catch and show error
            }
        }



        // UPDATE
        public async Task<bool> UpdatePolicyAsync(int id, PolicyCategoryDto dto)
        {
            var entity = await _context.PolicyCategories
                .FirstOrDefaultAsync(x =>
                    x.PolicyCategoryId == id &&
                    (x.IsDeleted == false || x.IsDeleted == null));

            if (entity == null)
                return false;

            entity.PolicyCategoryName = dto.PolicyCategoryName;
            entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;
            entity.CompanyId = dto.CompanyId;
            entity.RegionId = dto.RegionId;
            entity.ModifiedBy = dto.UserId;
            entity.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        // DELETE (SOFT)
        public async Task<bool> DeletePolicyAsync(int id, int userId)
        {
            var entity = await _context.PolicyCategories
                .FirstOrDefaultAsync(x =>
                    x.PolicyCategoryId == id &&
                    (x.IsDeleted == false || x.IsDeleted == null));

            if (entity == null)
                return false;

            entity.IsDeleted = true;
            entity.ModifiedBy = userId;
            entity.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        // MAPPER (LIKE GENDER)
        private static PolicyCategoryDto MapToDto(PolicyCategory x)
        {
            return new PolicyCategoryDto
            {
                PolicyCategoryId = x.PolicyCategoryId,
                PolicyCategoryName = x.PolicyCategoryName,
                Description = x.Description,
                IsActive = x.IsActive,
                CompanyId = x.CompanyId,
                RegionId = x.RegionId
            };
        }


        public async Task<bool> BulkUploadAsync(
     IFormFile file,
     int companyId,
     int regionId,
     int userId)
        {
            if (file == null || file.Length == 0)
                throw new Exception("File is empty");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            using var package = new ExcelPackage(stream);
            var sheet = package.Workbook.Worksheets[0];

            int rowCount = sheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++) // skip header
            {
                var name = sheet.Cells[row, 1].Text?.Trim();
                var isActiveText = sheet.Cells[row, 2].Text?.Trim();

                if (string.IsNullOrEmpty(name))
                    continue;

                bool isActive = isActiveText?.ToLower() != "false";

                var entity = new PolicyCategory
                {
                    PolicyCategoryName = name,
                    IsActive = isActive,
                    CompanyId = companyId,
                    RegionId = regionId,
                    CreatedBy = userId,
                    CreatedAt = DateTime.Now
                };

                _context.PolicyCategories.Add(entity);
            }

            await _context.SaveChangesAsync();
            return true;
        }




    }

}
