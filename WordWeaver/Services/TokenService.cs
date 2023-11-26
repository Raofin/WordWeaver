using System.IdentityModel.Tokens.Jwt;

namespace WordWeaver.Services
{
    public class TokenService(IHttpContextAccessor httpContextAccessor)
    {
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
