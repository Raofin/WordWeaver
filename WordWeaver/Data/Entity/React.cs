using System;
using System.Collections.Generic;

namespace WordWeaver.Data.Entity;

public partial class React
{
    public long ReactId { get; set; }

    public long? UserId { get; set; }

    public long? BlogId { get; set; }

    public long? ReactEnumId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Post? Blog { get; set; }

    public virtual ReactEnum? ReactEnum { get; set; }

    public virtual User? User { get; set; }
}
