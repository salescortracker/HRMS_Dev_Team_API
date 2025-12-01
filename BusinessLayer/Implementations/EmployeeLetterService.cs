
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Implementations
{
    public class EmployeeLetterService : IEmployeeLetterService
    {
        private readonly HRMSContext _context;

        public EmployeeLetterService(HRMSContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeLetterDto>> GetAllAsync()
        {
            return await _context.EmployeeLetters
                .Select(x => new EmployeeLetterDto
                {
                    Id = x.Id,
                    RegionId = x.RegionId,
                    CompanyId = x.CompanyId,
                    UserId = x.UserId,
                    DocumentTypeId = x.DocumentTypeId,
                    DocumentName = x.DocumentName,
                    EmployeeCode = x.EmployeeCode,
                    EmployeeName = x.EmployeeName,
                    IssuedDate = x.IssuedDate,
                    ValidityDate = x.ValidityDate,
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

        public async Task<IEnumerable<EmployeeLetterDto>> GetByUserIdAsync(int userId)
        {
            return await _context.EmployeeLetters
                .Where(x => x.UserId == userId)
                .Select(x => new EmployeeLetterDto
                {
                    Id = x.Id,
                    RegionId = x.RegionId,
                    CompanyId = x.CompanyId,
                    UserId = x.UserId,
                    DocumentTypeId = x.DocumentTypeId,
                    DocumentName = x.DocumentName,
                    EmployeeCode = x.EmployeeCode,
                    EmployeeName = x.EmployeeName,
                    IssuedDate = x.IssuedDate,
                    ValidityDate = x.ValidityDate,
                    FileName = x.FileName,
                    FilePath = x.FilePath,
                    Remarks = x.Remarks,
                    IsConfidential = x.IsConfidential,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<EmployeeLetterDto?> GetByIdAsync(int id)
        {
            return await _context.EmployeeLetters
                .Where(x => x.Id == id)
                .Select(x => new EmployeeLetterDto
                {
                    Id = x.Id,
                    RegionId = x.RegionId,
                    CompanyId = x.CompanyId,
                    UserId = x.UserId,
                    DocumentTypeId = x.DocumentTypeId,
                    DocumentName = x.DocumentName,
                    EmployeeCode = x.EmployeeCode,
                    EmployeeName = x.EmployeeName,
                    IssuedDate = x.IssuedDate,
                    ValidityDate = x.ValidityDate,
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

        public async Task<int> AddAsync(EmployeeLetterDto model)
        {
            var entity = new EmployeeLetter
            {
                RegionId = model.RegionId,
                CompanyId = model.CompanyId,
                UserId = model.UserId,
                DocumentTypeId = model.DocumentTypeId,
                DocumentName = model.DocumentName,
                EmployeeCode = model.EmployeeCode,
                EmployeeName = model.EmployeeName,
                IssuedDate = model.IssuedDate,
                ValidityDate = model.ValidityDate,
                FileName = model.FileName,
                FilePath = model.FilePath,
                Remarks = model.Remarks,
                IsConfidential = model.IsConfidential,
                CreatedBy = model.CreatedBy,
                CreatedAt = DateTime.Now
            };

            _context.EmployeeLetters.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(EmployeeLetterDto model)
        {
            var entity = await _context.EmployeeLetters.FindAsync(model.Id);
            if (entity == null)
                return false;

            entity.RegionId = model.RegionId;
            entity.CompanyId = model.CompanyId;
            entity.UserId = model.UserId;
            entity.DocumentTypeId = model.DocumentTypeId;
            entity.DocumentName = model.DocumentName;
            entity.EmployeeCode = model.EmployeeCode;
            entity.EmployeeName = model.EmployeeName;
            entity.IssuedDate = model.IssuedDate;
            entity.ValidityDate = model.ValidityDate;
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
            var entity = await _context.EmployeeLetters.FindAsync(id);
            if (entity == null)
                return false;

            _context.EmployeeLetters.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
