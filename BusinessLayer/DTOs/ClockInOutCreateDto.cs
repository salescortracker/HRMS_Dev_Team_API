namespace BusinessLayer.DTOs
{
    public class ClockInOutCreateDto
    {
        public int RegionId { get; set; }
        public int CompanyId { get; set; }
        public string EmployeeCode { get; set; } = null!;
        public string EmployeeName { get; set; } = null!;
        public string Department { get; set; } = null!;
        public DateTime AttendanceDate { get; set; }
        public string ActionType { get; set; } = null!; // "ClockIn" / "ClockOut"
        public string ActionTime { get; set; } = null!;   // "HH:mm"
    }
}
