namespace WordWeaver.Dtos;

public class UserListDto
{
    public long UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime? JoinedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public ProfileDto? UserDetails { get; set; }

    public List<UserRoleListDto> UserRoles { get; set; } = [];
}

public class UserRoleListDto
{
    public long UserRoleId { get; set; }

    public int? RoleId { get; set; }

    public string? RoleName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }
}
