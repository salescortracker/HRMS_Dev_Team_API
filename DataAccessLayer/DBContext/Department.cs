using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class Department
{
    public int DepartmentId { get; set; }

    public int CompanyId { get; set; }

    public int RegionId { get; set; }

    public string Description { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual Region Region { get; set; } = null!;
}
