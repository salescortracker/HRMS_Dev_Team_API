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
    public class MaritalStatusService : IMaritalStatusService
    {
        private readonly HRMSContext _context;

        public MaritalStatusService(HRMSContext context)
        {
            _context = context;
        }

        public async Task<List<MaritalStatusDto>> GetAllAsync()
        {
            return await _context.MaritalStatuses
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.MaritalStatusId)
                .Select(x => new MaritalStatusDto
                {
                    MaritalStatusId = x.MaritalStatusId,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    MaritalStatusName = x.MaritalStatusName,
                    Description = x.Description,
                    IsActive = x.IsActive
                })
                .ToListAsync();
        }

        public async Task<bool> CreateAsync(MaritalStatusDto dto, int userId)
        {
            var entity = new MaritalStatus
            {
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                MaritalStatusName = dto.MaritalStatusName,
                Description = dto.Description,
                IsActive = dto.IsActive,
                IsDeleted = false,
                CreatedBy = userId,
                CreatedAt = DateTime.Now
            };

            _context.MaritalStatuses.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(int id, MaritalStatusDto dto, int userId)
        {
            var entity = await _context.MaritalStatuses
                .FirstOrDefaultAsync(x => x.MaritalStatusId == id && !x.IsDeleted);

            if (entity == null) return false;

            entity.CompanyId = dto.CompanyId;
            entity.RegionId = dto.RegionId;
            entity.MaritalStatusName = dto.MaritalStatusName;
            entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;
            entity.ModifiedBy = userId;
            entity.ModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var entity = await _context.MaritalStatuses
                .FirstOrDefaultAsync(x => x.MaritalStatusId == id && !x.IsDeleted);

            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.ModifiedBy = userId;
            entity.ModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}