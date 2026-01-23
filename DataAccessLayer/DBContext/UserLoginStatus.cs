using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class UserLoginStatus
{
    public int UserId { get; set; }

    public bool MustChangePassword { get; set; }

    public virtual User User { get; set; } = null!;
}
