using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;


namespace BusinessLayer.Implementations
{
    public class EmployeePersonalService : IEmployeePersonalService
    {
        private readonly HRMSContext _context;

        public EmployeePersonalService(HRMSContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeePersonalDetailDto>> GetAllAsync()
        {
            return await _context.EmployeePersonalDetails
                .Select(x => new EmployeePersonalDetailDto
                {
                    Id = x.Id,
                    RegionId = x.RegionId,
                    CompanyId = x.CompanyId,
                    UserId = x.UserId,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    DateOfBirth = x.DateOfBirth != null ? DateTime.SpecifyKind(x.DateOfBirth.ToDateTime(TimeOnly.MinValue), DateTimeKind.Unspecified) : (DateTime?)null,
                    GenderId = x.GenderId,
                    MobileNumber = x.MobileNumber,
                    PersonalEmail = x.PersonalEmail,
                    PermanentAddress = x.PermanentAddress,
                    PresentAddress = x.PresentAddress,
                    Pannumber = x.Pannumber,
                    AadhaarNumber = x.AadhaarNumber,
                    ProfilePictureBase64 = x.ProfilePictureBase64,
                    ProfilePictureName = x.ProfilePictureName,
                    ProfilePicturePath = null, // path is not stored in entity; if you store it, map it
                    PassportNumber = x.PassportNumber,
                    PlaceOfBirth = x.PlaceOfBirth,
                    Uan = x.Uan,
                    BloodGroup = x.BloodGroup,
                    Citizenship = x.Citizenship,
                    Religion = x.Religion,
                    DrivingLicence = x.DrivingLicence,
                    MaritalStatusId = x.MaritalStatusId,
                    MarriageDate = x.MarriageDate != null ? DateTime.SpecifyKind(x.MarriageDate.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Unspecified) : (DateTime?)null,
                    WorkPhone = x.WorkPhone,
                    LinkedInProfile = x.LinkedInProfile,
                    PreviousExperienceText = x.PreviousExperienceText,
                    PreviousExperienceYears = x.PreviousExperienceYears,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedAt = x.ModifiedAt
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeePersonalDetailDto>> GetByUserIdAsync(int userId)
        {
            return await _context.EmployeePersonalDetails
                .Where(x => x.UserId == userId)
                .Select(x => new EmployeePersonalDetailDto
                {
                    Id = x.Id,
                    RegionId = x.RegionId,
                    CompanyId = x.CompanyId,
                    UserId = x.UserId,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    DateOfBirth = x.DateOfBirth != null ? DateTime.SpecifyKind(x.DateOfBirth.ToDateTime(TimeOnly.MinValue), DateTimeKind.Unspecified) : (DateTime?)null,
                    GenderId = x.GenderId,
                    MobileNumber = x.MobileNumber,
                    PersonalEmail = x.PersonalEmail,
                    PermanentAddress = x.PermanentAddress,
                    PresentAddress = x.PresentAddress,
                    Pannumber = x.Pannumber,
                    AadhaarNumber = x.AadhaarNumber,
                    ProfilePictureBase64 = x.ProfilePictureBase64,
                    ProfilePictureName = x.ProfilePictureName,
                    PassportNumber = x.PassportNumber,
                    PlaceOfBirth = x.PlaceOfBirth,
                    Uan = x.Uan,
                    BloodGroup = x.BloodGroup,
                    Citizenship = x.Citizenship,
                    Religion = x.Religion,
                    DrivingLicence = x.DrivingLicence,
                    MaritalStatusId = x.MaritalStatusId,
                    MarriageDate = x.MarriageDate != null ? DateTime.SpecifyKind(x.MarriageDate.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Unspecified) : (DateTime?)null,
                    WorkPhone = x.WorkPhone,
                    LinkedInProfile = x.LinkedInProfile,
                    PreviousExperienceText = x.PreviousExperienceText,
                    PreviousExperienceYears = x.PreviousExperienceYears,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedAt = x.ModifiedAt
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<EmployeePersonalDetailDto?> GetByIdAsync(int id)
        {
            return await _context.EmployeePersonalDetails
                .Where(x => x.Id == id)
                .Select(x => new EmployeePersonalDetailDto
                {
                    Id = x.Id,
                    RegionId = x.RegionId,
                    CompanyId = x.CompanyId,
                    UserId = x.UserId,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    DateOfBirth = x.DateOfBirth != null ? DateTime.SpecifyKind(x.DateOfBirth.ToDateTime(TimeOnly.MinValue), DateTimeKind.Unspecified) : (DateTime?)null,
                    GenderId = x.GenderId,
                    MobileNumber = x.MobileNumber,
                    PersonalEmail = x.PersonalEmail,
                    PermanentAddress = x.PermanentAddress,
                    PresentAddress = x.PresentAddress,
                    Pannumber = x.Pannumber,
                    AadhaarNumber = x.AadhaarNumber,
                    ProfilePictureBase64 = x.ProfilePictureBase64,
                    ProfilePictureName = x.ProfilePictureName,
                    PassportNumber = x.PassportNumber,
                    PlaceOfBirth = x.PlaceOfBirth,
                    Uan = x.Uan,
                    BloodGroup = x.BloodGroup,
                    Citizenship = x.Citizenship,
                    Religion = x.Religion,
                    DrivingLicence = x.DrivingLicence,
                    MaritalStatusId = x.MaritalStatusId,
                    MarriageDate = x.MarriageDate != null ? DateTime.SpecifyKind(x.MarriageDate.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Unspecified) : (DateTime?)null,
                    WorkPhone = x.WorkPhone,
                    LinkedInProfile = x.LinkedInProfile,
                    PreviousExperienceText = x.PreviousExperienceText,
                    PreviousExperienceYears = x.PreviousExperienceYears,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedAt = x.ModifiedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> AddAsync(EmployeePersonalDetailDto model)
        {
            var entity = new EmployeePersonalDetail
            {
                RegionId = model.RegionId,
                CompanyId = model.CompanyId,
                UserId = model.UserId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth.HasValue ? DateOnly.FromDateTime(model.DateOfBirth.Value) : default,
                GenderId = model.GenderId,
                MobileNumber = model.MobileNumber,
                PersonalEmail = model.PersonalEmail,
                PermanentAddress = model.PermanentAddress,
                PresentAddress = model.PresentAddress,
                Pannumber = model.Pannumber,
                AadhaarNumber = model.AadhaarNumber,
                // Profile picture handled by controller; you can set base64 here if provided:
                ProfilePictureBase64 = model.ProfilePictureBase64,
                ProfilePictureName = model.ProfilePictureName,
                PassportNumber = model.PassportNumber,
                PlaceOfBirth = model.PlaceOfBirth,
                Uan = model.Uan,
                BloodGroup = model.BloodGroup,
                Citizenship = model.Citizenship,
                Religion = model.Religion,
                DrivingLicence = model.DrivingLicence,
                MaritalStatusId = model.MaritalStatusId,
                MarriageDate = model.MarriageDate.HasValue ? DateOnly.FromDateTime(model.MarriageDate.Value) : (DateOnly?)null,
                WorkPhone = model.WorkPhone,
                LinkedInProfile = model.LinkedInProfile,
                PreviousExperienceText = model.PreviousExperienceText,
                PreviousExperienceYears = model.PreviousExperienceYears,
                CreatedBy = model.CreatedBy,
                CreatedAt = DateTime.Now
            };

            await _context.EmployeePersonalDetails.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(EmployeePersonalDetailDto model)
        {
            var entity = await _context.EmployeePersonalDetails.FindAsync(model.Id);
            if (entity == null) return false;

            entity.RegionId = model.RegionId;
            entity.CompanyId = model.CompanyId;
            entity.UserId = model.UserId;
            entity.FirstName = model.FirstName;
            entity.LastName = model.LastName;
            if (model.DateOfBirth.HasValue) entity.DateOfBirth = DateOnly.FromDateTime(model.DateOfBirth.Value);
            entity.GenderId = model.GenderId;
            entity.MobileNumber = model.MobileNumber;
            entity.PersonalEmail = model.PersonalEmail;
            entity.PermanentAddress = model.PermanentAddress;
            entity.PresentAddress = model.PresentAddress;
            entity.Pannumber = model.Pannumber;
            entity.AadhaarNumber = model.AadhaarNumber;
            if (!string.IsNullOrEmpty(model.ProfilePictureBase64)) entity.ProfilePictureBase64 = model.ProfilePictureBase64;
            if (!string.IsNullOrEmpty(model.ProfilePictureName)) entity.ProfilePictureName = model.ProfilePictureName;
            entity.PassportNumber = model.PassportNumber;
            entity.PlaceOfBirth = model.PlaceOfBirth;
            entity.Uan = model.Uan;
            entity.BloodGroup = model.BloodGroup;
            entity.Citizenship = model.Citizenship;
            entity.Religion = model.Religion;
            entity.DrivingLicence = model.DrivingLicence;
            entity.MaritalStatusId = model.MaritalStatusId;
            if (model.MarriageDate.HasValue) entity.MarriageDate = DateOnly.FromDateTime(model.MarriageDate.Value);
            entity.WorkPhone = model.WorkPhone;
            entity.LinkedInProfile = model.LinkedInProfile;
            entity.PreviousExperienceText = model.PreviousExperienceText;
            entity.PreviousExperienceYears = model.PreviousExperienceYears;
            entity.ModifiedBy = model.ModifiedBy;
            entity.ModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.EmployeePersonalDetails.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return false;

            _context.EmployeePersonalDetails.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
