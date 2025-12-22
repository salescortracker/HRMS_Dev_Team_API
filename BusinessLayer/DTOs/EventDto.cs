using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    //public class EventDto
    //{
    //    public int? EventId { get; set; }   // null = create
    //    public int EventTypeId { get; set; }
    //    public string EventName { get; set; }
    //    public DateOnly EventDate { get; set; }
    //    public string? Description { get; set; }
    //}


    public class EventDto
    {
        public int? EventId { get; set; }

        public int CompanyID { get; set; }
        public int RegionID { get; set; }
        public int UserID { get; set; }
        public int? RoleId { get; set; }

        public int EventTypeId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateOnly EventDate { get; set; }
        public string? Description { get; set; }
    }

}
