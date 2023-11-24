using System;
using System.Collections.Generic;

namespace WordWeaver.Data.Entity;

public partial class Role
{
    public long RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
