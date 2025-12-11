namespace BusinessLayer.DTOs
{
    public class LeaveTypeDto
    {
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = null!;
        public int LeaveDays { get; set; }
    }
}
