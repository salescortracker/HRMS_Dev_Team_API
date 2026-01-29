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
    public class EmployeeCertificationService : IEmployeeCertificationService
    {
        private readonly HRMSContext _context;

        public EmployeeCertificationService(HRMSContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeCertificationDto>> GetAllAsync()
        {
            return await _context.EmployeeCertifications
                .Select(x => new EmployeeCertificationDto
                {
                    CertificationId = x.CertificationId,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    UserId = x.UserId,

                    CertificationName = x.CertificationName,
                    CertificationTypeId = x.CertificationTypeId,
                    Description = x.Description,
                    DocumentPath = x.DocumentPath,

                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedDate = x.ModifiedDate
                   
                })
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeCertificationDto>> GetByUserIdAsync(int userId)
        {
            return await _context.EmployeeCertifications
                .Where(x => x.UserId == userId)
                .Select(x => new EmployeeCertificationDto
                {
                    CertificationId = x.CertificationId,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    UserId = x.UserId,
                    CertificationName = x.CertificationName,
                    CertificationTypeId = x.CertificationTypeId,
                    Description = x.Description,
                    DocumentPath = x.DocumentPath,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedDate = x.ModifiedDate,
                   
                })
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<EmployeeCertificationDto?> GetByIdAsync(int certificationId)
        {
            return await _context.EmployeeCertifications
                .Where(x => x.CertificationId == certificationId)
                .Select(x => new EmployeeCertificationDto
                {
                    CertificationId = x.CertificationId,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    UserId = x.UserId,
                    CertificationName = x.CertificationName,
                    CertificationTypeId = x.CertificationTypeId,
                    Description = x.Description,
                    DocumentPath = x.DocumentPath,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedDate = x.ModifiedDate,
                   
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> AddAsync(EmployeeCertificationDto model)
        {
            var entity = new EmployeeCertification
            {
                CompanyId = model.CompanyId,
                RegionId = model.RegionId,
                UserId = model.UserId,
                CertificationName = model.CertificationName,
                CertificationTypeId = model.CertificationTypeId,
                Description = model.Description,
                DocumentPath = model.DocumentPath,
                CreatedBy = model.CreatedBy,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            await _context.EmployeeCertifications.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.CertificationId;
        }

        public async Task<bool> UpdateAsync(EmployeeCertificationDto model)
        {
            var entity = await _context.EmployeeCertifications.FindAsync(model.CertificationId);

            if (entity == null) return false;

            entity.CertificationName = model.CertificationName;
            entity.CertificationTypeId = model.CertificationTypeId;
            entity.Description = model.Description;
            entity.DocumentPath = model.DocumentPath ?? entity.DocumentPath;
            entity.ModifiedBy = model.ModifiedBy;
            entity.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int certificationId)
        {
            var entity = await _context.EmployeeCertifications.FirstOrDefaultAsync(x => x.CertificationId == certificationId);

            if (entity == null) return false;

            _context.EmployeeCertifications.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CertificationTypeDto>> GetCertificationTypesAsync()
        {
            return await _context.CertificationTypes
                .Where(x => x.IsActive == true && x.IsDeleted == false)
                .Select(x => new CertificationTypeDto
                {
                    CertificationTypeId = x.CertificationTypeId,
                    CertificationTypeName = x.CertificationTypeName
                })
                .OrderBy(x => x.CertificationTypeName)
                .ToListAsync();
        }
    }
}
