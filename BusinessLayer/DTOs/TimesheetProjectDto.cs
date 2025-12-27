

namespace BusinessLayer.DTOs
{
    public class TimesheetProjectDto
    {
        public string ProjectName { get; set; } = null!;
        public string StartTime { get; set; } = null!;
        public string EndTime { get; set; } = null!;
        public int TotalMinutes { get; set; }
        public string TotalHoursText { get; set; } = null!;
        public int? OTMinutes { get; set; }
        public string? OTHoursText { get; set; }
    }
}
