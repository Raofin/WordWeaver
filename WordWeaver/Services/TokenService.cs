using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WordWeaver.Data.Entity;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.

namespace WordWeaver.Services
{
    public class TokenService(IHttpContextAccessor httpContextAccessor, IConfiguration config) : ITokenService
    {
        public string ClientIpAddress {
            get {
                return httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            }
        }

        public (string token, DateTime expiresAt) GenerateAuthToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]);
            var expiresAt = DateTime.UtcNow.AddDays(7);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),  // Subject (user ID)
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Iss, config["Jwt:Issuer"]),  // Issuer
                    new Claim(JwtRegisteredClaimNames.Aud, config["Jwt:Audience"]),  // Audience
                    new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(expiresAt).ToUnixTimeSeconds().ToString()),  // Expiration Time
                    new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()),  // Not Before
                    new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()),  // Issued At
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),  // JWT ID
                }
                .Union(user.UserRoles.Select(ur => new Claim(ClaimTypes.Role, ur.Role?.RoleName)))),  // Add roles

                Expires = expiresAt,  // Token expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return (tokenString, expiresAt);
        }

        public DecodedJwt DecodeJwt()
        {
            try
            {
                var authorizationHeader = httpContextAccessor.HttpContext.Request.Headers.Authorization.ToString();
                var token = authorizationHeader.Replace("Bearer ", "");

                var tokenHandler = new JwtSecurityTokenHandler();
                var decodedToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
                var claims = decodedToken?.Claims;

                return new DecodedJwt {
                    UserId = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value,
                    UniqueName = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value,
                    Email = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value,
                    Issuer = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Iss)?.Value,
                    Audience = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Aud)?.Value,
                    Expiration = UnixTimeSecondsToDatetime(claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value),
                    NotBefore = UnixTimeSecondsToDatetime(claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Nbf)?.Value),
                    IssuedAt = UnixTimeSecondsToDatetime(claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Iat)?.Value),
                    JwtId = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value,
                    Roles = claims.Where(c => c.Type == "role").Select(c => c.Value).ToList()
                };

            } catch (Exception ex)
            {
                throw new Exception("Error decoding JWT token", ex);
            }
        }

        private DateTime UnixTimeSecondsToDatetime(string? timestampString)
        {
            if (long.TryParse(timestampString, out long timestamp))
            {
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
                return dateTimeOffset.DateTime;
            }
            else
            {
                throw new ArgumentException("Invalid timestamp string");
            }
        }
    }

    public class DecodedJwt
    {
        public string? UserId { get; set; }
        public string? UniqueName { get; set; }
        public string? Email { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public DateTime? Expiration { get; set; }
        public DateTime? NotBefore { get; set; }
        public DateTime? IssuedAt { get; set; }
        public string? JwtId { get; set; }
        public List<string>? Roles { get; set; }
    }
}
