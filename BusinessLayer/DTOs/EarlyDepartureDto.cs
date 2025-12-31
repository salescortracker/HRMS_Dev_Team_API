using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{

    public class EarlyDepartureDto
    {
        public string EmployeeCode { get; set; } = null!;
        public string EmployeeName { get; set; } = null!;
        public int Department { get; set; }

        public DateOnly AttendanceDate { get; set; }

        public string ShiftName { get; set; } = null!;
        public TimeOnly ShiftEndTime { get; set; }
        public TimeOnly ClockOutTime { get; set; }

        public int EarlyByMinutes { get; set; }
    }

}
