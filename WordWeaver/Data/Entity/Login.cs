using System;
using System.Collections.Generic;

namespace WordWeaver.Data.Entity;

public partial class Login
{
    public long TokenId { get; set; }

    public long? UserId { get; set; }

    public string Token { get; set; } = null!;

    public string? IpAddress { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual User? User { get; set; }
}
