using Microsoft.AspNetCore.Http;


namespace BusinessLayer.DTOs
{
    public class EmployeeImageRequestDto
    {
        public int RegionId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }

        public IFormFile Image { get; set; } = null!;

        // these will be set in backend
        public string? FileName { get; set; }
        public string? FilePath { get; set; }

        public int? CreatedBy { get; set; }
    }
}
