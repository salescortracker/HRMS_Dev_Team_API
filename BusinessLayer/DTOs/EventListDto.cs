using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    //public class EventListDto
    //{
    //    public int EventId { get; set; }
    //    public string EventName { get; set; }
    //    public string EventTypeName { get; set; }
    //    public DateOnly EventDate { get; set; }

    //    public string? Description { get; set; }


    //}
    public class EventListDto
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string EventTypeName { get; set; } = string.Empty;
        public DateOnly EventDate { get; set; }
        public string? Description { get; set; }

        public int? RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }

    public class EventTypeDropdownDto
    {
        public int EventTypeId { get; set; }
        public string EventTypeName { get; set; } = string.Empty;
    }
}
