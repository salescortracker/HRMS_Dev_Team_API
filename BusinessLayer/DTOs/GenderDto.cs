using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class GenderDto
    {
        public int GenderId { get; set; }
        public string GenderName { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        // New Fields
        public int CompanyId { get; set; }
        public int RegionId { get; set; }
    }
}
