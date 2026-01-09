using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class CreateUpdateLeaveStatusDto
    {
        public int LeaveStatusID { get; set; }
        public int CompanyID { get; set; }
        public int RegionID { get; set; }
        public string LeaveStatusName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }

    }
}
