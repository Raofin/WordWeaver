using System;
using System.Collections.Generic;

namespace WordWeaver.Data.Entity;

public partial class Tag
{
    public long TagId { get; set; }

    public long? BlogId { get; set; }

    public long? TagEnumId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual Post? Blog { get; set; }

    public virtual TagEnum? TagEnum { get; set; }
}
