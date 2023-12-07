using System;
using System.Collections.Generic;

namespace WordWeaver.Data.Entity;

public partial class View
{
    public long BlogViewId { get; set; }

    public long? UserId { get; set; }

    public long? PostId { get; set; }

    public string? IpAddress { get; set; }

    public DateTime? ViewedAt { get; set; }

    public virtual Post? Post { get; set; }

    public virtual User? User { get; set; }
}
