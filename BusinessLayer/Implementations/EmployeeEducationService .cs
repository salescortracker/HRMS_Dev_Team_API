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
    public class EmployeeEducationService : IEmployeeEducationService
    {
        private readonly HRMSContext _context;

        public EmployeeEducationService(HRMSContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeEducationDto>> GetAllAsync()
        {
            return await _context.EmployeeEducations
                .Select(x => new EmployeeEducationDto
                {
                    EducationId = x.EducationId,
                    UserId = x.UserId,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    ModeOfStudyId = x.ModeOfStudyId,
                    Qualification = x.Qualification,
                    Specialization = x.Specialization,
                    Institution = x.Institution,
                    Board = x.Board,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Result = x.Result,
                    CertificateFilePath = x.CertificateFilePath,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedDate = x.ModifiedDate
                })
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeEducationDto>> GetByUserIdAsync(int userId)
        {
            return await _context.EmployeeEducations
                .Where(x => x.UserId == userId)
                .Select(x => new EmployeeEducationDto
                {
                    EducationId = x.EducationId,
                    UserId = x.UserId,
                    Qualification = x.Qualification,
                    Specialization = x.Specialization,
                    Institution = x.Institution,
                    Board = x.Board,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Result = x.Result,
                    CertificateFilePath = x.CertificateFilePath,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedDate = x.ModifiedDate,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    ModeOfStudyId = x.ModeOfStudyId
                })
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<EmployeeEducationDto?> GetByIdAsync(int educationId)
        {
            return await _context.EmployeeEducations
                .Where(x => x.EducationId == educationId)
                .Select(x => new EmployeeEducationDto
                {
                    EducationId = x.EducationId,
                    UserId = x.UserId,
                    Qualification = x.Qualification,
                    Specialization = x.Specialization,
                    Institution = x.Institution,
                    Board = x.Board,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Result = x.Result,
                    CertificateFilePath = x.CertificateFilePath,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedDate = x.ModifiedDate,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    ModeOfStudyId = x.ModeOfStudyId
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> AddAsync(EmployeeEducationDto model)
        {
            var entity = new EmployeeEducation
            {
                UserId = model.UserId,
                CompanyId = model.CompanyId,
                RegionId = model.RegionId,
                ModeOfStudyId = model.ModeOfStudyId,
                Qualification = model.Qualification,
                Specialization = model.Specialization,
                Institution = model.Institution,
                Board = model.Board,
                StartDate = model.StartDate ?? default,
                EndDate = model.EndDate ?? default,
                Result = model.Result,
                CertificateFilePath = model.CertificateFilePath,
                CreatedBy = model.CreatedBy,
                CreatedDate = DateTime.Now
            };

            await _context.EmployeeEducations.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.EducationId;
        }

        public async Task<bool> UpdateAsync(EmployeeEducationDto model)
        {
            var entity = await _context.EmployeeEducations.FindAsync(model.EducationId);
            if (entity == null)
                return false;

            entity.UserId = model.UserId;
            entity.CompanyId = model.CompanyId;
            entity.RegionId = model.RegionId;
            entity.ModeOfStudyId = model.ModeOfStudyId;
            entity.Qualification = model.Qualification;
            entity.Specialization = model.Specialization;
            entity.Institution = model.Institution;
            entity.Board = model.Board;
            entity.StartDate = model.StartDate ?? entity.StartDate;
            entity.EndDate = model.EndDate ?? entity.EndDate;
            entity.Result = model.Result ?? entity.Result;
            entity.CertificateFilePath = model.CertificateFilePath ?? entity.CertificateFilePath;
            entity.ModifiedBy = model.ModifiedBy;
            entity.ModifiedDate = DateTime.Now;

            _context.EmployeeEducations.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int educationId)
        {
            var entity = await _context.EmployeeEducations
                .FirstOrDefaultAsync(x => x.EducationId == educationId);

            if (entity == null)
                return false;

            _context.EmployeeEducations.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ModeOfStudyDto>> GetModeOfStudyListAsync()
        {
            return await _context.ModeOfStudies
                .Where(x => x.IsActive)
                .Select(x => new ModeOfStudyDto
                {
                    ModeOfStudyId = x.ModeOfStudyId,
                    ModeName = x.ModeName
                })
                .OrderBy(x => x.ModeName)
                .ToListAsync();
        }

    }
}
