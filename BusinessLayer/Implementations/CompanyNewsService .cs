using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class CompanyNewsService : ICompanyNewsService
    {
        private readonly HRMSContext _context;

        public CompanyNewsService(HRMSContext context)
        {
            _context = context;
        }

        // ================= HELPER =================

        private CompanyRegionInfo ResolveCompanyAndRegionByCategory(string category)
        {
            var result = _context.Users
                .Join(_context.RoleMasters,
                      u => u.RoleId,
                      r => r.RoleId,
                      (u, r) => new { u, r })
                .Where(x =>
                    x.r.RoleName == category &&
                    x.u.Status == "Active")
                .OrderByDescending(x => x.u.CreatedDate)
                .Select(x => new CompanyRegionInfo
                {
                    CompanyId = x.u.CompanyId,
                    RegionId = x.u.RegionId
                })
                .FirstOrDefault();

            if (result == null || result.CompanyId == 0)
                throw new Exception($"No active Company/Region found for category: {category}");

            return result;
        }

        // ================= CREATE =================

        public async Task<int> CreateAsync(CompanyNewsDto dto)
        {
            var companyRegion = ResolveCompanyAndRegionByCategory(dto.Category);

            var entity = new CompanyNews
            {
                Title = dto.Title,
                Category = dto.Category,
                Description = dto.Description,
                FromDate = DateOnly.FromDateTime(dto.FromDate),
                ToDate = DateOnly.FromDateTime(dto.ToDate),

                AttachmentName = dto.AttachmentName,
                AttachmentPath = dto.AttachmentPath,

                CompanyId = companyRegion.CompanyId,
                RegionId = companyRegion.RegionId,

                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            _context.CompanyNews.Add(entity);
            await _context.SaveChangesAsync();

            return entity.NewsId;
        }

        // ================= UPDATE =================

        public async Task UpdateAsync(CompanyNewsDto dto)
        {
            var entity = await _context.CompanyNews
                .FirstOrDefaultAsync(x => x.NewsId == dto.NewsId);

            if (entity == null)
                throw new Exception("Company News not found");

            var companyRegion = ResolveCompanyAndRegionByCategory(dto.Category);

            entity.Title = dto.Title;
            entity.Category = dto.Category;
            entity.Description = dto.Description;
            entity.FromDate = DateOnly.FromDateTime(dto.FromDate);
            entity.ToDate = DateOnly.FromDateTime(dto.ToDate);

            // 🔹 update file only if new file uploaded
            if (!string.IsNullOrEmpty(dto.AttachmentName))
            {
                entity.AttachmentName = dto.AttachmentName;
                entity.AttachmentPath = dto.AttachmentPath;
            }

            entity.CompanyId = companyRegion.CompanyId;
            entity.RegionId = companyRegion.RegionId;

            entity.UpdatedBy = dto.UpdatedBy;
            entity.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        // ================= GET ALL =================

        public async Task<List<CompanyNewsDto>> GetAllAsync(int companyId, int regionId)
        {
            var query = _context.CompanyNews.Where(x => x.IsActive);

            if (companyId > 0)
                query = query.Where(x => x.CompanyId == companyId);

            if (regionId > 0)
                query = query.Where(x => x.RegionId == regionId);

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new CompanyNewsDto
                {
                    NewsId = x.NewsId,
                    Title = x.Title,
                    Category = x.Category,
                    Description = x.Description,
                    FromDate = x.FromDate.ToDateTime(TimeOnly.MinValue),
                    ToDate = x.ToDate.ToDateTime(TimeOnly.MinValue),

                    // ✅ REQUIRED FOR FRONTEND
                    AttachmentName = x.AttachmentName,
                    AttachmentPath = x.AttachmentPath,

                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    CreatedBy = x.CreatedBy
                })
                .ToListAsync();
        }

        // ================= DELETE =================

        public async Task DeleteAsync(int newsId)
        {
            var entity = await _context.CompanyNews
                .FirstOrDefaultAsync(x => x.NewsId == newsId);

            if (entity == null)
                throw new Exception("News not found");

            _context.CompanyNews.Remove(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<List<CompanyNewsDto>> GetForSuperAdminAsync(string category)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            return await _context.CompanyNews
                .Where(x =>
                    x.IsActive &&
                    x.Category == category &&
                    x.FromDate <= today &&
                    x.ToDate >= today)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new CompanyNewsDto
                {
                    Title = x.Title,
                    Category = x.Category,
                    Description = x.Description,
                    DisplayDate = x.CreatedAt
                })
                .ToListAsync();
        }
    }
}
