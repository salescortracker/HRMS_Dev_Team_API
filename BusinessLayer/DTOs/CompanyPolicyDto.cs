using Microsoft.AspNetCore.Http;

namespace BusinessLayer.DTOs
{
    public class CompanyPolicyDto
    {
        public int PolicyId { get; set; }

        public int CompanyId { get; set; }
        public int RegionId { get; set; }

        public string Title { get; set; } = null!;
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public DateTime EffectiveDate { get; set; }

        public string? Description { get; set; }

        // File
        public IFormFile? File { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
    }
}
