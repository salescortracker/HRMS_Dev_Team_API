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
    public class EmployeeReferenceService : IEmployeeReferenceService
    {
        private readonly HRMSContext _context;

        public EmployeeReferenceService(HRMSContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeReferenceDto>> GetAllAsync()
        {
            var q = _context.EmployeeReferences
                .Select(r => new EmployeeReferenceDto
                {
                    ReferenceId = r.ReferenceId,
                    RegionId = r.RegionId,
                    CompanyId = r.CompanyId,
                    Name = r.Name,
                    TitleOrDesignation = r.TitleOrDesignation,
                    CompanyName = r.CompanyName,
                    EmailId = r.EmailId,
                    MobileNumber = r.MobileNumber,
                    CreatedAt = r.CreatedAt,
                    CreatedBy = r.CreatedBy,
                    ModifiedAt = r.ModifiedAt,
                    ModifiedBy = r.ModifiedBy,
                    UserId = r.UserId
                });

            return await q.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<EmployeeReferenceDto>> GetByUserIdAsync(int userId)
        {
            var q = _context.EmployeeReferences
                .Where(r => r.UserId == userId)
                .Select(r => new EmployeeReferenceDto
                {
                    ReferenceId = r.ReferenceId,
                    RegionId = r.RegionId,
                    CompanyId = r.CompanyId,
                    Name = r.Name,
                    TitleOrDesignation = r.TitleOrDesignation,
                    CompanyName = r.CompanyName,
                    EmailId = r.EmailId,
                    MobileNumber = r.MobileNumber,
                    CreatedAt = r.CreatedAt,
                    CreatedBy = r.CreatedBy,
                    ModifiedAt = r.ModifiedAt,
                    ModifiedBy = r.ModifiedBy,
                    UserId = r.UserId
                });

            return await q.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task<EmployeeReferenceDto?> GetByIdAsync(int referenceId)
        {
            var r = await _context.EmployeeReferences
                .Where(x => x.ReferenceId == referenceId)
                .Select(r => new EmployeeReferenceDto
                {
                    ReferenceId = r.ReferenceId,
                    RegionId = r.RegionId,
                    CompanyId = r.CompanyId,
                    Name = r.Name,
                    TitleOrDesignation = r.TitleOrDesignation,
                    CompanyName = r.CompanyName,
                    EmailId = r.EmailId,
                    MobileNumber = r.MobileNumber,
                    CreatedAt = r.CreatedAt,
                    CreatedBy = r.CreatedBy,
                    ModifiedAt = r.ModifiedAt,
                    ModifiedBy = r.ModifiedBy,
                    UserId = r.UserId
                })
                .FirstOrDefaultAsync();

            return r;
        }

        public async Task<int> AddAsync(EmployeeReferenceDto model)
        {
            // map DTO -> EF entity
            var entity = new EmployeeReference
            {
                RegionId = model.RegionId,
                CompanyId = model.CompanyId,
                Name = model.Name,
                TitleOrDesignation = model.TitleOrDesignation,
                CompanyName = model.CompanyName,
                EmailId = model.EmailId,
                MobileNumber = model.MobileNumber,
                CreatedAt = DateTime.Now,
                CreatedBy = model.CreatedBy,
                UserId = model.UserId
            };

            await _context.EmployeeReferences.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity.ReferenceId;
        }

        public async Task<bool> UpdateAsync(EmployeeReferenceDto model)
        {
            var entity = await _context.EmployeeReferences.FindAsync(model.ReferenceId);
            if (entity == null)
                return false;

            entity.RegionId = model.RegionId;
            entity.CompanyId = model.CompanyId;
            entity.Name = model.Name;
            entity.TitleOrDesignation = model.TitleOrDesignation;
            entity.CompanyName = model.CompanyName;
            entity.EmailId = model.EmailId;
            entity.MobileNumber = model.MobileNumber;
            entity.ModifiedAt = DateTime.Now;
            entity.ModifiedBy = model.ModifiedBy;
            // userId typically shouldn't change; but update if provided
            entity.UserId = model.UserId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int referenceId)
        {
            var entity = await _context.EmployeeReferences.FirstOrDefaultAsync(x => x.ReferenceId == referenceId);
            if (entity == null) return false;

            _context.EmployeeReferences.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
