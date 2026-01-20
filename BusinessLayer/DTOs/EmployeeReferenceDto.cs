using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class EmployeeReferenceDto
    {
        public int ReferenceId { get; set; }

        [Required]
        public int RegionId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string TitleOrDesignation { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string EmailId { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        [RegularExpression(@"^[0-9+ ]*$", ErrorMessage = "Invalid phone number")]
        public string MobileNumber { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } // set by service on add
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public int? ModifiedBy { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
