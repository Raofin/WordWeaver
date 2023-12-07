using System;
using System.Collections.Generic;

namespace WordWeaver.Data.Entity;

public partial class UserDetail
{
    public long ProfileId { get; set; }

    public long? UserId { get; set; }

    public string? FullName { get; set; }

    public string? Bio { get; set; }

    public DateTime? Birthday { get; set; }

    public string? Country { get; set; }

    public string? Website { get; set; }

    public string? Phone { get; set; }

    /// <summary>
    /// Fk from Cloudfiles
    /// </summary>
    public long? AvatarFileId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual User? User { get; set; }
}
