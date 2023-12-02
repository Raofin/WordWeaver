using System;
using System.Collections.Generic;

namespace WordWeaver.Data.Entity;

public partial class ReactEnum
{
    public long ReactEnumId { get; set; }

    public string? ReactName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<React> Reacts { get; set; } = new List<React>();
}
