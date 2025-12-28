using System;

namespace DataAccessLayer.DBContext
{
    public class ClockInOut
    {
        public int ClockInOutId { get; set; }
        public int RegionId { get; set; }
        public int CompanyId { get; set; }
        public string EmployeeCode { get; set; } = null!;
        public string EmployeeName { get; set; } = null!;
        public string Department { get; set; } = null!;
        public DateOnly AttendanceDate { get; set; }
        public string? ActionType { get; set; }
        public TimeSpan ActionTime { get; set; }
        public string Status { get; set; } = null!;
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
