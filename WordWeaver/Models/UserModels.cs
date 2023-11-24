using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WordWeaver.Models
{
    public class LoginModel
    {
        public string UsernameOrEmail { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class RegistrationModel
    {
        [StringLength(20, MinimumLength = 6)]
        public string Username { get; set; } = null!;

        [StringLength(20, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [EmailAddress]
        public string Email { get; set; } = null!;

        public Roles Role { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Roles
    {
        Admin = 1,
        User = 2
    }
}
