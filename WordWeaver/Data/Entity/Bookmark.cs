using System;
using System.Collections.Generic;

namespace WordWeaver.Data.Entity;

public partial class Bookmark
{
    public long BookmarkId { get; set; }

    public long? UserId { get; set; }

    public long? PostId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual Post? Post { get; set; }

    public virtual User? User { get; set; }
}
