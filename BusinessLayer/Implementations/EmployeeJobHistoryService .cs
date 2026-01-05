using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Implementations
{
    public class EmployeeJobHistoryService : IEmployeeJobHistoryService
    {
        private readonly HRMSContext _context;

        public EmployeeJobHistoryService(HRMSContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeJobHistoryDto>> GetAllAsync()
        {
            return await _context.EmployeeJobHistories
                .Select(x => new EmployeeJobHistoryDto
                {
                    Id = x.Id,
                    Employer = x.Employer,
                    JobTitle = x.JobTitle,

                    // DateOnly → DateTime
                    FromDate = x.FromDate.ToDateTime(TimeOnly.MinValue),
                    ToDate = x.ToDate.ToDateTime(TimeOnly.MinValue),

                    LastCTC = x.LastCtc,
                    Website = x.Website,
                    EmployeeCode = x.EmployeeCode,
                    ReasonForLeaving = x.ReasonForLeaving,
                    UploadDocumentPath = x.UploadDocument,

                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    UserId = x.UserId,

                    CreatedBy = x.CreatedBy ?? 0,
                    CreatedAt = x.CreatedAt,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedAt = x.ModifiedAt
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeJobHistoryDto>> GetByUserIdAsync(int userId)
        {
            return await _context.EmployeeJobHistories
                .Where(x => x.UserId == userId)
                .Select(x => new EmployeeJobHistoryDto
                {
                    Id = x.Id,
                    Employer = x.Employer,
                    JobTitle = x.JobTitle,

                    FromDate = x.FromDate.ToDateTime(TimeOnly.MinValue),
                    ToDate = x.ToDate.ToDateTime(TimeOnly.MinValue),

                    LastCTC = x.LastCtc,
                    Website = x.Website,
                    EmployeeCode = x.EmployeeCode,
                    ReasonForLeaving = x.ReasonForLeaving,
                    UploadDocumentPath = x.UploadDocument,

                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    UserId = x.UserId,

                    CreatedBy = x.CreatedBy ?? 0,
                    CreatedAt = x.CreatedAt,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedAt = x.ModifiedAt
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<EmployeeJobHistoryDto?> GetByIdAsync(int id)
        {
            return await _context.EmployeeJobHistories
                .Where(x => x.Id == id)
                .Select(x => new EmployeeJobHistoryDto
                {
                    Id = x.Id,
                    Employer = x.Employer,
                    JobTitle = x.JobTitle,

                    FromDate = x.FromDate.ToDateTime(TimeOnly.MinValue),
                    ToDate = x.ToDate.ToDateTime(TimeOnly.MinValue),

                    LastCTC = x.LastCtc,
                    Website = x.Website,
                    EmployeeCode = x.EmployeeCode,
                    ReasonForLeaving = x.ReasonForLeaving,
                    UploadDocumentPath = x.UploadDocument,

                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    UserId = x.UserId,

                    CreatedBy = x.CreatedBy ?? 0,
                    CreatedAt = x.CreatedAt,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedAt = x.ModifiedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> AddAsync(EmployeeJobHistoryDto model)
        {
            var entity = new EmployeeJobHistory
            {
                Employer = model.Employer,
                JobTitle = model.JobTitle,

                // DateTime → DateOnly
                FromDate = DateOnly.FromDateTime(model.FromDate),
                ToDate = DateOnly.FromDateTime(model.ToDate),

                LastCtc = model.LastCTC,
                Website = model.Website,
                EmployeeCode = model.EmployeeCode,
                ReasonForLeaving = model.ReasonForLeaving,

                UploadDocument = model.UploadDocumentPath,

                CompanyId = model.CompanyId,
                RegionId = model.RegionId,
                UserId = model.UserId,

                CreatedBy = model.CreatedBy,
                CreatedAt = DateTime.Now
            };

            await _context.EmployeeJobHistories.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(EmployeeJobHistoryDto model)
        {
            var entity = await _context.EmployeeJobHistories.FindAsync(model.Id);
            if (entity == null) return false;

            entity.Employer = model.Employer;
            entity.JobTitle = model.JobTitle;

            // DateTime → DateOnly
            entity.FromDate = DateOnly.FromDateTime(model.FromDate);
            entity.ToDate = DateOnly.FromDateTime(model.ToDate);

            entity.LastCtc = model.LastCTC;
            entity.Website = model.Website;
            entity.EmployeeCode = model.EmployeeCode;
            entity.ReasonForLeaving = model.ReasonForLeaving;

            if (!string.IsNullOrEmpty(model.UploadDocumentPath))
            {
                entity.UploadDocument = model.UploadDocumentPath;
            }

            entity.CompanyId = model.CompanyId;
            entity.RegionId = model.RegionId;
            entity.UserId = model.UserId;

            entity.ModifiedBy = model.ModifiedBy;
            entity.ModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.EmployeeJobHistories.FindAsync(id);
            if (entity == null) return false;

            _context.EmployeeJobHistories.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
