
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Implementations
{
    public class EmployeeFormService : IEmployeeFormService
    {
        private readonly HRMSContext _context;

        public EmployeeFormService(HRMSContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeFormDto>> GetAllAsync()
        {
            return await _context.EmployeeForms
                .Select(x => new EmployeeFormDto
                {
                    Id = x.Id,
                    RegionId = x.RegionId,
                    CompanyId = x.CompanyId,
                    UserId = x.UserId,
                    DocumentTypeId = x.DocumentTypeId,
                    DocumentName = x.DocumentName,
                    EmployeeCode = x.EmployeeCode,
                    IssueDate = x.IssueDate,
                    FileName = x.FileName,
                    FilePath = x.FilePath,
                    Remarks = x.Remarks,
                    IsConfidential = x.IsConfidential,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedAt = x.ModifiedAt
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeFormDto>> GetByUserIdAsync(int userId)
        {
            return await _context.EmployeeForms
                .Where(x => x.UserId == userId)
                .Select(x => new EmployeeFormDto
                {
                    Id = x.Id,
                    RegionId = x.RegionId,
                    CompanyId = x.CompanyId,
                    UserId = x.UserId,
                    DocumentTypeId = x.DocumentTypeId,
                    DocumentName = x.DocumentName,
                    EmployeeCode = x.EmployeeCode,
                    IssueDate = x.IssueDate,
                    FileName = x.FileName,
                    FilePath = x.FilePath,
                    Remarks = x.Remarks,
                    IsConfidential = x.IsConfidential,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedAt = x.ModifiedAt
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<EmployeeFormDto?> GetByIdAsync(int id)
        {
            return await _context.EmployeeForms
                .Where(x => x.Id == id)
                .Select(x => new EmployeeFormDto
                {
                    Id = x.Id,
                    RegionId = x.RegionId,
                    CompanyId = x.CompanyId,
                    UserId = x.UserId,
                    DocumentTypeId = x.DocumentTypeId,
                    DocumentName = x.DocumentName,
                    EmployeeCode = x.EmployeeCode,
                    IssueDate = x.IssueDate,
                    FileName = x.FileName,
                    FilePath = x.FilePath,
                    Remarks = x.Remarks,
                    IsConfidential = x.IsConfidential,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedAt = x.ModifiedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> AddAsync(EmployeeFormDto model)
        {
            var entity = new EmployeeForm
            {
                RegionId = model.RegionId,
                CompanyId = model.CompanyId,
                UserId = model.UserId,
                DocumentTypeId = model.DocumentTypeId,
                DocumentName = model.DocumentName,
                EmployeeCode = model.EmployeeCode,
                IssueDate = model.IssueDate,
                FileName = model.FileName,
                FilePath = model.FilePath,
                Remarks = model.Remarks,
                IsConfidential = model.IsConfidential,
                CreatedBy = model.CreatedBy,
                CreatedAt = DateTime.Now
            };

            await _context.EmployeeForms.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(EmployeeFormDto model)
        {
            var entity = await _context.EmployeeForms.FindAsync(model.Id);
            if (entity == null)
                return false;

            entity.RegionId = model.RegionId;
            entity.CompanyId = model.CompanyId;
            entity.UserId = model.UserId;
            entity.DocumentTypeId = model.DocumentTypeId;
            entity.DocumentName = model.DocumentName;
            entity.EmployeeCode = model.EmployeeCode;
            entity.IssueDate = model.IssueDate;
            entity.FileName = model.FileName;
            entity.FilePath = model.FilePath ?? entity.FilePath;
            entity.Remarks = model.Remarks;
            entity.IsConfidential = model.IsConfidential;
            entity.ModifiedBy = model.ModifiedBy;
            entity.ModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.EmployeeForms.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                return false;

            _context.EmployeeForms.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
