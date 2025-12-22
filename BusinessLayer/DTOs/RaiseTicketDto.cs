using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class RaiseTicketCreateDto
    {
      
        public int CompanyID { get; set; }
        public int RegionID { get; set; }
        public int DepartmentID { get; set; }

        public int CategoryID { get; set; }

        public string SubjectIssue { get; set; } = string.Empty;

        public int Priority { get; set; }

        public string Status { get; set; } = string.Empty;


        public string Description { get; set; } = string.Empty;

        public string? UploadPicPath { get; set; } = string.Empty;
        public int CreatedBy { get; set; }

    }

    public class RaiseTicketUpdateDto
    {
        public int RaiseTicketID { get; set; }
        public int DepartmentID { get; set; }

        public int CategoryID { get; set; }

        public string SubjectIssue { get; set; } = string.Empty;

        public int Priority { get; set; }

        public string Status { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string? UploadPicPath { get; set; } = string.Empty;
        public int ModifiedBy { get; set; }



    }

    public class RaiseTicketReadDto
    {
        
        public int RaiseTicketID { get; set; }
        public int DepartmentID { get; set; }

        public int CategoryID { get; set; }

        public string SubjectIssue { get; set; } = string.Empty;

        public int Priority { get; set; }

        public string Description { get; set; } = string.Empty;

        public string? UploadPicPath { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }
    }
}
