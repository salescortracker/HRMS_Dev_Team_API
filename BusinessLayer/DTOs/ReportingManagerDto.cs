namespace BusinessLayer.DTOs
{
    public class ReportingManagerDto
    {
        public int UserId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public string? ManagerEmail { get; set; }
    }
}
