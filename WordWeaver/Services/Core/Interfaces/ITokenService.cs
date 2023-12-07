using WordWeaver.Data.Entity;
using WordWeaver.Dtos;

namespace WordWeaver.Services.Core.Interfaces;

public interface ITokenService
{
    (string token, DateTime expiresAt) GenerateAuthToken(User user);

    DecodedJwt DecodeJwt();
}

public interface IAuthenticatedUser
{
    string? IpAddress { get; }

    long UserId { get; }

    long? UserIdNullable { get; }
}