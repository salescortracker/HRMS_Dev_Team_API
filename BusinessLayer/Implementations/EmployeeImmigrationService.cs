using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;

public class EmployeeImmigrationService : IEmployeeImmigrationService
{
    private readonly HRMSContext _context;

    public EmployeeImmigrationService(HRMSContext context)
    {
        _context = context;
    }

    public async Task<List<EmployeeImmigrationDto>> GetAllImmigrationAsync()
    {
        return await _context.EmployeeImmigrations
            .Include(v => v.VisaTypeMaster)
        .Include(s => s.WorkAuthStatusMaster)
            .Select( x => new EmployeeImmigrationDto
            {
                ImmigrationId = x.ImmigrationId,
                CompanyId = x.CompanyId,
                RegionId = x.RegionId,
                UserId = x.UserId,
               
               EmployeeId = x.EmployeeId,
                FullName = x.FullName,
                DateOfBirth = x.DateOfBirth,
                Nationality = x.Nationality,
                PassportNumber = x.PassportNumber,
                PassportExpiryDate = x.PassportExpiryDate,

                VisaTypeId = x.VisaTypeId,
                VisaTypeName = x.VisaTypeMaster.VisaTypeName,     // ⭐ Added

                StatusId = x.StatusId,
                StatusName = x.WorkAuthStatusMaster.StatusName,  // ⭐ Added

                VisaNumber = x.VisaNumber,
                VisaIssuingCountry = x.VisaIssuingCountry,
                VisaIssueDate = x.VisaIssueDate,
                VisaExpiryDate = x.VisaExpiryDate,
                EmployerName = x.EmployerName,
                EmployerAddress = x.EmployerAddress,
                ContactPerson = x.ContactPerson,
                EmployerContact = x.EmployerContact,
                Remarks = x.Remarks
            })
            .ToListAsync();
    }

    public async Task<EmployeeImmigrationDto> GetByIdImmigrationAsync(int id)
    {
        return await _context.EmployeeImmigrations
            .Include(v => v.VisaTypeMaster)
            .Include(s => s.WorkAuthStatusMaster)
            .Where(x => x.ImmigrationId == id)
            .Select(x => new EmployeeImmigrationDto
            {
                ImmigrationId = x.ImmigrationId,
                CompanyId = x.CompanyId,
                RegionId = x.RegionId,
                UserId = x.UserId,
                EmployeeId = x.EmployeeId,
                FullName = x.FullName,
                DateOfBirth = x.DateOfBirth,
                Nationality = x.Nationality,
                PassportNumber = x.PassportNumber,
                PassportExpiryDate = x.PassportExpiryDate,

                VisaTypeId = x.VisaTypeId,
                VisaTypeName = x.VisaTypeMaster.VisaTypeName,  

                StatusId = x.StatusId,
                StatusName = x.WorkAuthStatusMaster.StatusName, 

                VisaNumber = x.VisaNumber,
                VisaIssuingCountry = x.VisaIssuingCountry,
                VisaIssueDate = x.VisaIssueDate,
                VisaExpiryDate = x.VisaExpiryDate,
                EmployerName = x.EmployerName,
                EmployerAddress = x.EmployerAddress,
                ContactPerson = x.ContactPerson,
                EmployerContact = x.EmployerContact,
                Remarks = x.Remarks
            })
            .FirstOrDefaultAsync();
    }


    public async Task<bool> CreateImmigrationAsync(EmployeeImmigrationDto dto)
    {
        var entity = new EmployeeImmigration
        {
            CompanyId = dto.CompanyId,
            RegionId = dto.RegionId,
            UserId = dto.UserId,
            EmployeeId=dto.EmployeeId,          
            FullName = dto.FullName,
            DateOfBirth = dto.DateOfBirth,
            Nationality = dto.Nationality,
            PassportNumber = dto.PassportNumber,
            PassportExpiryDate = dto.PassportExpiryDate,
            VisaTypeId = dto.VisaTypeId,
            StatusId = dto.StatusId,
            VisaNumber = dto.VisaNumber,
            VisaIssuingCountry = dto.VisaIssuingCountry,
            VisaIssueDate = dto.VisaIssueDate,
            VisaExpiryDate = dto.VisaExpiryDate,
            EmployerName = dto.EmployerName,
            EmployerAddress = dto.EmployerAddress,
            ContactPerson = dto.ContactPerson,
            EmployerContact = dto.EmployerContact,
            Remarks = dto.Remarks,
            CreatedDate = DateTime.Now,
            // ⭐ ADD THESE THREE LINES
            PassportCopyPath = dto.PassportCopyPath,
            VisaCopyPath = dto.VisaCopyPath,
            OtherDocumentsPath = dto.OtherDocumentsPath
        };

        _context.EmployeeImmigrations.Add(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateImmigrationAsync(EmployeeImmigrationDto dto)
    {
        var entity = await _context.EmployeeImmigrations.FindAsync(dto.ImmigrationId);

        if (entity == null) return false;

        entity.CompanyId = dto.CompanyId;
        entity.RegionId = dto.RegionId;
        entity.UserId = dto.UserId;
        entity.EmployeeId = dto.EmployeeId;
        entity.FullName = dto.FullName;
        entity.DateOfBirth = dto.DateOfBirth;
        entity.Nationality = dto.Nationality;
        entity.PassportNumber = dto.PassportNumber;
        entity.PassportExpiryDate = dto.PassportExpiryDate;
        entity.VisaTypeId = dto.VisaTypeId;
        entity.StatusId = dto.StatusId;
        entity.VisaNumber = dto.VisaNumber;
        entity.VisaIssuingCountry = dto.VisaIssuingCountry;
        entity.VisaIssueDate = dto.VisaIssueDate;
        entity.VisaExpiryDate = dto.VisaExpiryDate;
        entity.EmployerName = dto.EmployerName;
        entity.EmployerAddress = dto.EmployerAddress;
        entity.ContactPerson = dto.ContactPerson;
        entity.EmployerContact = dto.EmployerContact;
        entity.Remarks = dto.Remarks;
        // ⭐ ADD THESE LINES
        if (dto.PassportCopyPath != null)
            entity.PassportCopyPath = dto.PassportCopyPath;

        if (dto.VisaCopyPath != null)
            entity.VisaCopyPath = dto.VisaCopyPath;

        if (dto.OtherDocumentsPath != null)
            entity.OtherDocumentsPath = dto.OtherDocumentsPath;
        entity.ModifiedDate = DateTime.Now;

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteImmigrationAsync(int id)
    {
        var entity = await _context.EmployeeImmigrations.FindAsync(id);
        if (entity == null) return false;

        _context.EmployeeImmigrations.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<List<VisaTypeDto>> GetVisaTypesAsync()
    {
        return await _context.VisaTypeMasters
            .Select(v => new VisaTypeDto
            {
                VisaTypeId = v.VisaTypeId,
                CompanyId = v.CompanyId,
                RegionId = v.RegionId,
                VisaTypeName = v.VisaTypeName
            })
            .ToListAsync();
    }

    public async Task<List<WorkAuthStatusDto>> GetStatusListAsync()
    {
        return await _context.WorkAuthStatusMasters
            .Select(s => new WorkAuthStatusDto
            {
                StatusId = s.StatusId,
                CompanyId = s.CompanyId,
                RegionId = s.RegionId,
                StatusName = s.StatusName
            })
            .ToListAsync();
    }
}
