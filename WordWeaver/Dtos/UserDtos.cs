using System.ComponentModel.DataAnnotations;
using WordWeaver.Validations;

namespace WordWeaver.Dtos;

#region ### Auth Dtos ###

public class LoginDto
{
    public string UsernameOrEmail { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class RegistrationDto
{
    [StringLength(20, MinimumLength = 2)]
    public string Username { get; set; } = null!;

    [StringLength(20, MinimumLength = 2)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [EmailAddress]
    public string Email { get; set; } = null!;
    public string Otp { get; set; } = null!;
}

public class VerifyOtpDto
{
    [EmailAddress]
    public string Email { get; set; } = null!;
    public string Otp { get; set; } = null!;
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

#endregion ### Auth Dtos ###

#region ### User Detail Dtos ###

public class UserDetailsDto
{
    public long ProfileId { get; set; }

    public long UserId { get; set; }

    public string? FullName { get; set; }

    public string? Bio { get; set; }

    [MinimumAge(9)]
    public DateTime? Birthday { get; set; }

    public string? Country { get; set; }

    public string? Website { get; set; }

    public string? Phone { get; set; }

    public List<SocialDto>? Socials { get; set; }

    public bool? IsActive { get; set; }
}

public class ProfileDto
{
    public long ProfileId { get; set; }

    public long UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Bio { get; set; }

    public DateTime? Birthday { get; set; }

    public string? Country { get; set; }

    public string? Phone { get; set; }

    public string? Website { get; set; }

    public long? AvatarFileId { get; set; }

    public DateTime? JoinedDate { get; set; }

    public List<SocialDto>? Socials { get; set; }
}

public class SocialDto
{
    public long SocialId { get; set; }

    public string? SocialName { get; set; }

    public string? SocialUrl { get; set; }
}

#endregion ### User Detail Dtos ###