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
        public int Department { get; set; }
        public string Departments { get; set; }
        public DateTime AttendanceDate { get; set; }

        public string ActionType { get; set; } = null!;
        public string ActionTime { get; set; } = null!;

        // ✅ NEW
        public string? ClockInTime { get; set; }
        public string? ClockOutTime { get; set; }
        public int TotalMinutes { get; set; }
        public string TotalDuration { get; set; } = "00:00";

        public string Status { get; set; } = null!;
    }


}
