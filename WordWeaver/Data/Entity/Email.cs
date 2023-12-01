using System;
using System.Collections.Generic;

namespace WordWeaver.Data.Entity;

public partial class Email
{
    public long EmailId { get; set; }

    public string EmailTo { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string Body { get; set; } = null!;

    public string? CcRecipients { get; set; }

    public string? BccRecipients { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }
}
