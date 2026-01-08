using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.DTOs
{
    public class PolicyCategoryDto
    {
        // For update/read, PolicyCategoryId will be used
        public int? PolicyCategoryId { get; set; }

     
        public int CompanyId { get; set; }

        public int RegionId { get; set; }

        
        public string PolicyCategoryName { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public int UserId { get; set; }  

        public DateTime? CreatedAt { get; set; }  
        public DateTime? ModifiedAt { get; set; }
    }
}
