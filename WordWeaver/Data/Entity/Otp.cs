using System;
using System.Collections.Generic;

namespace WordWeaver.Data.Entity;

public partial class Otp
{
    public long OtpId { get; set; }

    public string Email { get; set; } = null!;

    public string OtpValue { get; set; } = null!;

    public DateTime? ExpiresAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }
}
