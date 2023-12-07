using System;
using System.Collections.Generic;

namespace WordWeaver.Data.Entity;

public partial class User
{
    public long UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<CloudFile> CloudFiles { get; set; } = new List<CloudFile>();

    public virtual ICollection<Error> Errors { get; set; } = new List<Error>();

    public virtual ICollection<Login> Logins { get; set; } = new List<Login>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<React> Reacts { get; set; } = new List<React>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual ICollection<View> Views { get; set; } = new List<View>();
}
