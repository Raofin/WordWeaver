﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WordWeaver.Data.Entity;
using WordWeaver.Dtos;
using WordWeaver.Services.Core.Interfaces;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.

namespace WordWeaver.Services.Core;

public class TokenService(IHttpContextAccessor httpContextAccessor, IAppSettingsService appSettings) : ITokenService, IAuthenticatedUser
{
    #region ### ITokenService ###

    public (string token, DateTime expiresAt) GenerateAuthToken(User user)
    {
        var key = Encoding.ASCII.GetBytes(appSettings.JwtKey);
        var expiresAt = DateTime.UtcNow.AddDays(7);

        var tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),  // Subject (user ID)
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Iss, appSettings.JwtIssuer),  // Issuer
                new Claim(JwtRegisteredClaimNames.Aud, appSettings.JwtAudience),  // Audience
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

    #endregion ### ITokenService ###

    #region ### IAuthenticatedUser ###

    public string IpAddress {
        get {
            try
            {
                return httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            } catch (Exception ex)
            {
                throw new Exception("Error getting client IP address", ex);
            }
        }
    }

    public long UserId {
        get {

            try
            {
                var authorizationHeader = httpContextAccessor.HttpContext.Request.Headers.Authorization.ToString();
                var token = authorizationHeader.Replace("Bearer ", "");

                var tokenHandler = new JwtSecurityTokenHandler();
                var decodedToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
                var claims = decodedToken?.Claims;

                var userId = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

                return long.Parse(userId);

            } catch (Exception ex)
            {
                throw new Exception("Invalid authorization.", ex);
            }
        }
    }

    public long? UserIdNullable {
        get {

            try
            {
                var authorizationHeader = httpContextAccessor.HttpContext.Request.Headers.Authorization.ToString();
                
                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    return null;
                }

                var token = authorizationHeader.Replace("Bearer ", "");

                var tokenHandler = new JwtSecurityTokenHandler();
                var decodedToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
                var claims = decodedToken?.Claims;

                var userIdClaim = claims?.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);

                if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
                {
                    return null;
                }

                return userId;

            } catch (Exception ex)
            {
                throw new Exception("Invalid authorization.", ex);
            }
        }
    }

    #endregion ### IAuthenticatedUser ###
}
