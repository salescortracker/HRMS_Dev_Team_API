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
    public class EmployeeEmergencyContactService : IEmployeeEmergencyContactService
    {
        private readonly HRMSContext _context;

        public EmployeeEmergencyContactService(HRMSContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeEmergencyContactDto>> GetAllAsync()
        {
            var query = from e in _context.EmployeeEmergencyContacts
                        join r in _context.Relationships on e.RelationshipId equals r.RelationshipId into gj
                        from rel in gj.DefaultIfEmpty()
                        select new EmployeeEmergencyContactDto
                        {
                            EmergencyContactId = e.EmergencyContactId,
                           
                            UserId = e.UserId,
                            CompanyId = e.CompanyId,
                            RegionId = e.RegionId,
                            ContactName = e.ContactName,
                            RelationshipId = e.RelationshipId,
                            Relationship = rel != null ? rel.RelationshipName : null,
                            PhoneNumber = e.PhoneNumber,
                            AlternatePhone = e.AlternatePhone,
                            Email = e.Email,
                            Address = e.Address,
                            CreatedBy = e.CreatedBy,
                            CreatedDate = e.CreatedDate,
                            ModifiedBy = e.ModifiedBy,
                            ModifiedDate = e.ModifiedDate
                        };

            return await query.OrderByDescending(x => x.CreatedDate).ToListAsync();
        }

        public async Task<IEnumerable<EmployeeEmergencyContactDto>> GetByUserIdAsync(int userId)
        {
            var query = from e in _context.EmployeeEmergencyContacts
                        where e.UserId == userId
                        join r in _context.Relationships on e.RelationshipId equals r.RelationshipId into gj
                        from rel in gj.DefaultIfEmpty()
                        select new EmployeeEmergencyContactDto
                        {
                            EmergencyContactId = e.EmergencyContactId,
                           
                            UserId = e.UserId,
                            CompanyId = e.CompanyId,
                            RegionId = e.RegionId,
                            ContactName = e.ContactName,
                            RelationshipId = e.RelationshipId,
                            Relationship = rel != null ? rel.RelationshipName : null,
                            PhoneNumber = e.PhoneNumber,
                            AlternatePhone = e.AlternatePhone,
                            Email = e.Email,
                            Address = e.Address,
                            CreatedBy = e.CreatedBy,
                            CreatedDate = e.CreatedDate,
                            ModifiedBy = e.ModifiedBy,
                            ModifiedDate = e.ModifiedDate
                        };

            return await query.OrderByDescending(x => x.CreatedDate).ToListAsync();
        }

        public async Task<EmployeeEmergencyContactDto?> GetByIdAsync(int emergencyContactId)
        {
            var query = from e in _context.EmployeeEmergencyContacts
                        where e.EmergencyContactId == emergencyContactId
                        join r in _context.Relationships on e.RelationshipId equals r.RelationshipId into gj
                        from rel in gj.DefaultIfEmpty()
                        select new EmployeeEmergencyContactDto
                        {
                            EmergencyContactId = e.EmergencyContactId,
                           
                            UserId = e.UserId,
                            CompanyId = e.CompanyId,
                            RegionId = e.RegionId,
                            ContactName = e.ContactName,
                            RelationshipId = e.RelationshipId,
                            Relationship = rel != null ? rel.RelationshipName : null,
                            PhoneNumber = e.PhoneNumber,
                            AlternatePhone = e.AlternatePhone,
                            Email = e.Email,
                            Address = e.Address,
                            CreatedBy = e.CreatedBy,
                            CreatedDate = e.CreatedDate,
                            ModifiedBy = e.ModifiedBy,
                            ModifiedDate = e.ModifiedDate
                        };

            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> AddAsync(EmployeeEmergencyContactDto model)
        {
            // Prefer relationship name from master if ID provided
            string relationshipName = model.Relationship ?? string.Empty;
            if (model.RelationshipId > 0)
            {
                var relFromMaster = await _context.Relationships
                    .Where(r => r.RelationshipId == model.RelationshipId)
                    .Select(r => r.RelationshipName)
                    .FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(relFromMaster))
                    relationshipName = relFromMaster;
            }

            var entity = new EmployeeEmergencyContact
            {
               
                UserId = model.UserId,
                CompanyId = model.CompanyId,
                RegionId = model.RegionId,
                ContactName = model.ContactName ?? string.Empty,
                RelationshipId = model.RelationshipId,
                // if your EF model has a Relationship string column, set it; else it's optional
                // assuming EF model only has RelationshipId (but you had navigation property)
                PhoneNumber = model.PhoneNumber,
                AlternatePhone = model.AlternatePhone,
                Email = model.Email,
                Address = model.Address,
                CreatedBy = model.CreatedBy,
                CreatedDate = model.CreatedDate ?? DateTime.Now
            };

            await _context.EmployeeEmergencyContacts.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.EmergencyContactId;
        }

        public async Task<bool> UpdateAsync(EmployeeEmergencyContactDto model)
        {
            var entity = await _context.EmployeeEmergencyContacts.FindAsync(model.EmergencyContactId);
            if (entity == null)
                return false;

           
            entity.UserId = model.UserId;
            entity.CompanyId = model.CompanyId;
            entity.RegionId = model.RegionId;
            entity.ContactName = model.ContactName ?? entity.ContactName;
            entity.RelationshipId = model.RelationshipId != 0 ? model.RelationshipId : entity.RelationshipId;

            // try update relationship name if needed (same pattern as family)
            if (model.RelationshipId > 0)
            {
                var relFromMaster = await _context.Relationships
                    .Where(r => r.RelationshipId == model.RelationshipId)
                    .Select(r => r.RelationshipName)
                    .FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(relFromMaster))
                {
                    // If you have a Relationship string column on the EF model, update it.
                    // entity.Relationship = relFromMaster; // uncomment if present
                }
            }
            else if (!string.IsNullOrEmpty(model.Relationship))
            {
                // entity.Relationship = model.Relationship; // uncomment if present
            }

            entity.PhoneNumber = model.PhoneNumber ?? entity.PhoneNumber;
            entity.AlternatePhone = model.AlternatePhone ?? entity.AlternatePhone;
            entity.Email = model.Email ?? entity.Email;
            entity.Address = model.Address ?? entity.Address;
            entity.ModifiedBy = model.ModifiedBy;
            entity.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int emergencyContactId)
        {
            var entity = await _context.EmployeeEmergencyContacts.FirstOrDefaultAsync(x => x.EmergencyContactId == emergencyContactId);
            if (entity == null)
                return false;

            _context.EmployeeEmergencyContacts.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RelationshipDto>> GetRelationshipListAsync()
        {
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
    }
}
