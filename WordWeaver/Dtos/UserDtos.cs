using System.ComponentModel.DataAnnotations;

namespace WordWeaver.Dtos;
public class LoginDto
{
    public string UsernameOrEmail { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class RegistrationDto
{
    [StringLength(20, MinimumLength = 6)]
    public string Username { get; set; } = null!;

    [StringLength(20, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [EmailAddress]
    public string Email { get; set; } = null!;
}

public class UserDto
{
    public long UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<UserRoleDto> UserRoles { get; set; } = new List<UserRoleDto>();
}

public class UserRoleDto
{
    public long UserRoleId { get; set; }

    public int? RoleId { get; set; }

    public long? UserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual RoleDto? Role { get; set; } = new RoleDto();
}

public class RoleDto
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }
}