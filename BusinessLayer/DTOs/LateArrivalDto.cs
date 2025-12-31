using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class LateArrivalDto
    {
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public DateOnly AttendanceDate { get; set; }
        public TimeOnly ShiftStartTime { get; set; }
        public TimeOnly AllowedClockInTime { get; set; }
        public TimeOnly? ClockInTime { get; set; }
        public int LateByMinutes { get; set; }
        public string Status { get; set; }
    }


}
