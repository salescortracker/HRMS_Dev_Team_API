using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class Auditlog
{
    public long AuditId { get; set; }

    public int? UserId { get; set; }

    public string? ActionType { get; set; }

    public string? ActionDetails { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifedAt { get; set; }

    public int? ModifiedBy { get; set; }
}
