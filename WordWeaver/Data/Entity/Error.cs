using System;
using System.Collections.Generic;

namespace WordWeaver.Data.Entity;

public partial class Error
{
    public long ErrorId { get; set; }

    public DateTime? Timestamp { get; set; }

    public bool? IsHandled { get; set; }

    public string? Message { get; set; }

    public string? Exception { get; set; }

    public string? RequestPath { get; set; }

    public string? HttpMethod { get; set; }

    public string? IpAddress { get; set; }

    public long? UserId { get; set; }

    public virtual User? User { get; set; }
}
