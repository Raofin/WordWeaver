using System;
using System.Collections.Generic;

namespace WordWeaver.Data.Entity;

public partial class Social
{
    public long SocialId { get; set; }

    public long? UserId { get; set; }

    public string? SocialName { get; set; }

    public string? SocialUrl { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual User? User { get; set; }
}
