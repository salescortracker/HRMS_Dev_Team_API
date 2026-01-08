namespace BusinessLayer.DTOs
{
    public class EventDto
    {
        public int EventTypeId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string? Description { get; set; }
        public int CompanyId { get; set; }
        public int RegionId { get; set; }
    }

    public class EventTypeDto
    {
        public int EventTypeId { get; set; }
        public string EventTypeName { get; set; } = string.Empty;
    }
}
