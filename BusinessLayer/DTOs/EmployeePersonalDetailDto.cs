
using Microsoft.AspNetCore.Http;

namespace BusinessLayer.DTOs
{
    public class EmployeePersonalDetailDto
    {
        public int Id { get; set; }
        public int RegionId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; } // use DateTime in DTO for easier binding
        public int GenderId { get; set; }
        public string MobileNumber { get; set; } = string.Empty;
        public string PersonalEmail { get; set; } = string.Empty;
        public string PermanentAddress { get; set; } = string.Empty;
        public string PresentAddress { get; set; } = string.Empty;
        public string Pannumber { get; set; } = string.Empty;
        public string AadhaarNumber { get; set; } = string.Empty;

        // Profile picture upload:
        public IFormFile? ProfilePicture { get; set; }
        public string? ProfilePicturePath { get; set; }   // relative path saved on server
        public string? ProfilePictureBase64 { get; set; } // optional: store base64 if you prefer

        public string? ProfilePictureName { get; set; }

        public string? PassportNumber { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? Uan { get; set; }
        public string BloodGroup { get; set; } = string.Empty;
        public string? Citizenship { get; set; }
        public string? Religion { get; set; }
        public string? DrivingLicence { get; set; }
        public int MaritalStatusId { get; set; }
        public DateTime? MarriageDate { get; set; }
        public string? WorkPhone { get; set; }
        public string? LinkedInProfile { get; set; }
        public string? PreviousExperienceText { get; set; }
        public byte? PreviousExperienceYears { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
