using System;
using System.Collections.Generic;

namespace WordWeaver.Data.Entity;

public partial class TagEnum
{
    public long TagEnumId { get; set; }

    public string TagName { get; set; } = null!;

    public long? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
