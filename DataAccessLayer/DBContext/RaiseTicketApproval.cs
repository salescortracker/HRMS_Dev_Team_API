using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class RaiseTicketApproval
{
    public int RaiseTicketApprovalId { get; set; }

    public int RaiseTicketId { get; set; }

    public string? Status { get; set; }

    public DateTime? RaisedOn { get; set; }

    public int? ManagerStatus { get; set; }

    public string? ManagerComments { get; set; }

    public int ManagerId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
