
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;

using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Implementations
{
    public class EmployeDocument : IEmployeDocument
    {
        private readonly HRMSContext _context;

        public EmployeDocument(HRMSContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeDocumentDto>> GetAllAsync()
        {
            return await _context.EmployeeDocuments
                .Select(x => new EmployeeDocumentDto
                {
                    Id = x.Id,
                    RegionId = x.RegionId,
                    CompanyId = x.CompanyId,
                    UserId = x.UserId,
                    DocumentTypeId = x.DocumentTypeId,
                    DocumentName = x.DocumentName,
                    DocumentNumber = x.DocumentNumber,
                    IssuedDate = x.IssuedDate,
                    ExpiryDate = x.ExpiryDate,
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

        public async Task<IEnumerable<EmployeeDocumentDto>> GetByUserIdAsync(int userId)
        {
            return await _context.EmployeeDocuments
                .Where(x => x.UserId == userId)
                .Select(x => new EmployeeDocumentDto
                {
                    Id = x.Id,
                    RegionId = x.RegionId,
                    CompanyId = x.CompanyId,
                    UserId = x.UserId,
                    DocumentTypeId = x.DocumentTypeId,
                    DocumentName = x.DocumentName,
                    DocumentNumber = x.DocumentNumber,
                    IssuedDate = x.IssuedDate,
                    ExpiryDate = x.ExpiryDate,
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

        public async Task<EmployeeDocumentDto?> GetByIdAsync(int id)
        {
            return await _context.EmployeeDocuments
                .Where(x => x.Id == id)
                .Select(x => new EmployeeDocumentDto
                {
                    Id = x.Id,
                    RegionId = x.RegionId,
                    CompanyId = x.CompanyId,
                    UserId = x.UserId,
                    DocumentTypeId = x.DocumentTypeId,
                    DocumentName = x.DocumentName,
                    DocumentNumber = x.DocumentNumber,
                    IssuedDate = x.IssuedDate,
                    ExpiryDate = x.ExpiryDate,
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

        public async Task<int> AddAsync(EmployeeDocumentDto model)
        {
            var entity = new EmployeeDocument
            {
                RegionId = model.RegionId,
                CompanyId = model.CompanyId,
                UserId = model.UserId,
                DocumentTypeId = model.DocumentTypeId,
                DocumentName = model.DocumentName,
                DocumentNumber = model.DocumentNumber,
                IssuedDate = model.IssuedDate,
                ExpiryDate = model.ExpiryDate,
                FileName = model.FileName,
                FilePath = model.FilePath,
                Remarks = model.Remarks,
                IsConfidential = model.IsConfidential,
                CreatedBy = model.CreatedBy,
                CreatedAt = DateTime.Now
            };

            await _context.EmployeeDocuments.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(EmployeeDocumentDto model)
        {
            var entity = await _context.EmployeeDocuments.FindAsync(model.Id);
            if (entity == null)
                return false;

            entity.RegionId = model.RegionId;
            entity.CompanyId = model.CompanyId;
            entity.UserId = model.UserId;
            entity.DocumentTypeId = model.DocumentTypeId;
            entity.DocumentName = model.DocumentName;
            entity.DocumentNumber = model.DocumentNumber;
            entity.IssuedDate = model.IssuedDate;
            entity.ExpiryDate = model.ExpiryDate;
            entity.FileName = model.FileName ?? entity.FileName;
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
            var entity = await _context.EmployeeDocuments.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                return false;

            _context.EmployeeDocuments.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<DocumentTypeDto>> GetActiveDocumentTypesAsync()
        {
            return await _context.DocumentTypes
                .Where(d => d.IsActive == true)
                .Select(d => new DocumentTypeDto
                {
                    Id = d.Id,
                    TypeName = d.TypeName,
                    IsActive = d.IsActive
                })
                .ToListAsync();
        }

    }
}
