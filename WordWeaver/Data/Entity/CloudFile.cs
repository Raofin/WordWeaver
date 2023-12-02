using System;
using System.Collections.Generic;

namespace WordWeaver.Data.Entity;

/// <summary>
/// Uploaded By
/// </summary>
public partial class CloudFile
{
    public long FileId { get; set; }

    public string Name { get; set; } = null!;

    public string Extension { get; set; } = null!;

    public string Size { get; set; } = null!;

    public DateTime? UploadedAt { get; set; }

    public long? UserId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual User? User { get; set; }
}
