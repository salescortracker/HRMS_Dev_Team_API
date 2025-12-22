using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class RaiseTicket
{
    public int RaiseTicketId { get; set; }

    public int CompanyId { get; set; }

    public int RegionId { get; set; }

    public int EmployeeId { get; set; }

    public int DepartmentId { get; set; }

    public int? CategoryId { get; set; }

    public string? SubjectIssue { get; set; }

    public int? Priority { get; set; }

    public string? Description { get; set; }

    public string? UploadPicPath { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string Status { get; set; } = null!;
}
