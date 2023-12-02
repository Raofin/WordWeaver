using System;
using System.Collections.Generic;

namespace WordWeaver.Data.Entity;

public partial class Post
{
    public long PostId { get; set; }

    public long? UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Text { get; set; } = null!;

    /// <summary>
    /// Medias
    /// </summary>
    public string? FileIds { get; set; }

    public bool? IsPublished { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<React> Reacts { get; set; } = new List<React>();

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public virtual ICollection<View> Views { get; set; } = new List<View>();
}
