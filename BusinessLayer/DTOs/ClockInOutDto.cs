using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
   
    public class ClockInOutDto
    {
        public int ClockInOutId { get; set; }
        public int RegionId { get; set; }
        public int CompanyId { get; set; }

        public string EmployeeCode { get; set; } = null!;
        public string EmployeeName { get; set; } = null!;
        public string Department { get; set; } = null!;

        public DateTime AttendanceDate { get; set; }

        public string ActionType { get; set; } = null!;

        // send as "HH:mm"
        public string ActionTime { get; set; } = null!;

        public string Status { get; set; } = null!;
    }


}
