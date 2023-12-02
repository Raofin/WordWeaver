using System;
using System.Collections.Generic;

namespace WordWeaver.Data.Entity;

public partial class Comment
{
    public long CommentId { get; set; }

    public long? UserId { get; set; }

    public string Text { get; set; } = null!;

    public long ParentId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }
}
