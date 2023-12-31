﻿using System;
using System.Collections.Generic;

namespace WordWeaver.Data.Entity;

public partial class UserRole
{
    public long UserRoleId { get; set; }

    public int? RoleId { get; set; }

    public long? UserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual RoleEnum? Role { get; set; }

    public virtual User? User { get; set; }
}
