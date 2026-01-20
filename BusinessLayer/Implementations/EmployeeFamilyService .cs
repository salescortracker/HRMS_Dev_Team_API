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
    public class EmployeeFamilyService : IEmployeeFamilyService
    {
        private readonly HRMSContext _context;

        public EmployeeFamilyService(HRMSContext context)
        {
            _context = context;
        }

        // NOTE: EmployeeFamilyDetail.DateOfBirth is DateOnly in your EF model,
        // while DTO uses DateTime. Convert explicitly.
        public async Task<IEnumerable<EmployeeFamilyDto>> GetAllAsync()
        {
            var query = from f in _context.EmployeeFamilyDetails
                        join r in _context.Relationships
                            on f.RelationshipId equals r.RelationshipId into gj
                        from rel in gj.DefaultIfEmpty()
                        select new EmployeeFamilyDto
                        {
                            FamilyId = f.FamilyId,
                            UserId = f.UserId,
                            CompanyId = f.CompanyId,       // use CompanyId (camel-case) to match EF model
                            RegionId = f.RegionId,         // use RegionId
                            Name = f.Name,
                            RelationshipId = f.RelationshipId,
                            Relationship = rel != null ? rel.RelationshipName : null, // left-join rel name
                            // Convert DateOnly -> DateTime
                            DateOfBirth = f.DateOfBirth.ToDateTime(System.TimeOnly.MinValue),
                            Gender = f.Gender,
                            GenderId = f.GenderId,
                            Occupation = f.Occupation,
                            Phone = f.Phone,
                            Address = f.Address,
                            IsDependent = f.IsDependent,
                            CreatedBy = f.CreatedBy,
                            CreatedDate = f.CreatedDate,
                            ModifiedBy = f.ModifiedBy,
                            ModifiedDate = f.ModifiedDate
                        };

            return await query.OrderByDescending(x => x.CreatedDate).ToListAsync();
        }

        public async Task<IEnumerable<EmployeeFamilyDto>> GetByUserIdAsync(int userId)
        {
            var query = from f in _context.EmployeeFamilyDetails
                        where f.UserId == userId
                        join r in _context.Relationships
                            on f.RelationshipId equals r.RelationshipId into gj
                        from rel in gj.DefaultIfEmpty()
                        select new EmployeeFamilyDto
                        {
                            FamilyId = f.FamilyId,
                            UserId = f.UserId,
                            CompanyId = f.CompanyId,
                            RegionId = f.RegionId,
                            Name = f.Name,
                            RelationshipId = f.RelationshipId,
                            Relationship = rel != null ? rel.RelationshipName : null,
                            DateOfBirth = f.DateOfBirth.ToDateTime(System.TimeOnly.MinValue),
                            Gender = f.Gender,
                            GenderId = f.GenderId,
                            Occupation = f.Occupation,
                            Phone = f.Phone,
                            Address = f.Address,
                            IsDependent = f.IsDependent,
                            CreatedBy = f.CreatedBy,
                            CreatedDate = f.CreatedDate,
                            ModifiedBy = f.ModifiedBy,
                            ModifiedDate = f.ModifiedDate
                        };

            return await query.OrderByDescending(x => x.CreatedDate).ToListAsync();
        }

        public async Task<EmployeeFamilyDto?> GetByIdAsync(int familyId)
        {
            var query = from f in _context.EmployeeFamilyDetails
                        where f.FamilyId == familyId
                        join r in _context.Relationships
                            on f.RelationshipId equals r.RelationshipId into gj
                        from rel in gj.DefaultIfEmpty()
                        select new EmployeeFamilyDto
                        {
                            FamilyId = f.FamilyId,
                            UserId = f.UserId,
                            CompanyId = f.CompanyId,
                            RegionId = f.RegionId,
                            Name = f.Name,
                            RelationshipId = f.RelationshipId,
                            Relationship = rel != null ? rel.RelationshipName : null,
                            DateOfBirth = f.DateOfBirth.ToDateTime(System.TimeOnly.MinValue),
                            Gender = f.Gender,
                            GenderId = f.GenderId,
                            Occupation = f.Occupation,
                            Phone = f.Phone,
                            Address = f.Address,
                            IsDependent = f.IsDependent,
                            CreatedBy = f.CreatedBy,
                            CreatedDate = f.CreatedDate,
                            ModifiedBy = f.ModifiedBy,
                            ModifiedDate = f.ModifiedDate
                        };

            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> AddAsync(EmployeeFamilyDto model)
        {
            // Start with provided relationship string if any
            string relationshipName = model.Relationship ?? string.Empty;

            // If caller provided a RelationshipId, try to get the master value
            if (model.RelationshipId.HasValue)
            {
                var relFromMaster = await _context.Relationships
                    .Where(r => r.RelationshipId == model.RelationshipId.Value)
                    .Select(r => r.RelationshipName)
                    .FirstOrDefaultAsync();

                // If we found a name in master, use it; otherwise keep whatever model.Relationship had
                if (!string.IsNullOrEmpty(relFromMaster))
                    relationshipName = relFromMaster;
            }

            // Ensure non-null (DB column is NOT NULL)
            if (relationshipName == null)
                relationshipName = string.Empty;

            var entity = new EmployeeFamilyDetail
            {
                UserId = model.UserId,
                CompanyId = model.CompanyId,
                RegionId = model.RegionId,
                Name = model.Name,
                Relationship = relationshipName,            // <-- guaranteed non-null
                RelationshipId = model.RelationshipId,
                DateOfBirth = DateOnly.FromDateTime(model.DateOfBirth),
                Gender = model.Gender,
                GenderId = model.GenderId,
                Occupation = model.Occupation,
                Phone = model.Phone,
                Address = model.Address,
                IsDependent = model.IsDependent,
                CreatedBy = model.CreatedBy,
                CreatedDate = DateTime.Now
            };

            await _context.EmployeeFamilyDetails.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity.FamilyId;
        }

        public async Task<bool> UpdateAsync(EmployeeFamilyDto model)
        {
            var entity = await _context.EmployeeFamilyDetails.FindAsync(model.FamilyId);
            if (entity == null)
                return false;

            entity.UserId = model.UserId;
            entity.CompanyId = model.CompanyId;
            entity.RegionId = model.RegionId;
            entity.Name = model.Name;

            // Update Relationship name if RelationshipId provided (preferred)
            if (model.RelationshipId.HasValue)
            {
                var relFromMaster = await _context.Relationships
                    .Where(r => r.RelationshipId == model.RelationshipId.Value)
                    .Select(r => r.RelationshipName)
                    .FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(relFromMaster))
                    entity.Relationship = relFromMaster;
                else
                    entity.Relationship = model.Relationship ?? entity.Relationship ?? string.Empty;
            }
            else if (!string.IsNullOrEmpty(model.Relationship))
            {
                entity.Relationship = model.Relationship;
            }
            // else keep existing entity.Relationship

            if (model.DateOfBirth != default)
                entity.DateOfBirth = DateOnly.FromDateTime(model.DateOfBirth);

            entity.Gender = model.Gender ?? entity.Gender;
            entity.GenderId = model.GenderId ?? entity.GenderId;
            entity.Occupation = model.Occupation ?? entity.Occupation;
            entity.Phone = model.Phone ?? entity.Phone;
            entity.Address = model.Address ?? entity.Address;
            entity.IsDependent = model.IsDependent;
            entity.ModifiedBy = model.ModifiedBy;
            entity.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int familyId)
        {
            var entity = await _context.EmployeeFamilyDetails.FirstOrDefaultAsync(x => x.FamilyId == familyId);
            if (entity == null)
                return false;

            _context.EmployeeFamilyDetails.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RelationshipDto>> GetRelationshipListAsync()
        {
            // Use the Relationships DbSet to get active relationships
            return await _context.Relationships
                .Where(r => r.IsActive && !r.IsDeleted)
                .Select(r => new RelationshipDto
                {
                    RelationshipID = r.RelationshipId,
                    RelationshipName = r.RelationshipName
                })
                .OrderBy(r => r.RelationshipName)
                .ToListAsync();
        }
        public async Task<IEnumerable<DropdownGenderDto>> GetGenderListAsync()
        {
            return await _context.Genders
                .Where(g => g.IsActive && !g.IsDeleted)
                .Select(g => new DropdownGenderDto
                {
                    GenderId = g.GenderId,
                    GenderName = g.GenderName
                })
                .OrderBy(g => g.GenderName)
                .ToListAsync();
        }


    }
}
