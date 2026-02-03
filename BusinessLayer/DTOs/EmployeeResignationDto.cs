using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class EmployeeResignationDto
    {
        public string EmployeeId { get; set; }
        public string ResignationType { get; set; }   // "Retirement"
        public string NoticePeriod { get; set; }      // "90"
        public DateTime? LastWorkingDay { get; set; }
        public string ResignationReason { get; set; }

        public int? CompanyId { get; set; }
        public int? RegionId { get; set; }
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
    }
}
