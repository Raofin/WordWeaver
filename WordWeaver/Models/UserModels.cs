using System.ComponentModel.DataAnnotations;

namespace WordWeaver.Models;

public class LoginModel
{
    public string UsernameOrEmail { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class RegistrationModel
{
    [StringLength(20, MinimumLength = 6)]
    public string Username { get; set; } = null!;

    [StringLength(20, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [EmailAddress]
    public string Email { get; set; } = null!;
}
