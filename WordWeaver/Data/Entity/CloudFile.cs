using System;
using System.Collections.Generic;

namespace WordWeaver.Data.Entity;

public partial class CloudFile
{
    public long FileId { get; set; }

    public string Name { get; set; } = null!;

    public string Extension { get; set; } = null!;

    public string Size { get; set; } = null!;

    public DateTime? UploadedAt { get; set; }

    public long? UploadedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }
}
