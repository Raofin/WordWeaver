using System;
using System.Collections.Generic;

namespace WordWeaver.Data.Entity;

public partial class Token
{
    public long TokenId { get; set; }

    public long? UserId { get; set; }

    public string TokenValue { get; set; } = null!;

    public DateTime? ExpiresAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual User? User { get; set; }
}
