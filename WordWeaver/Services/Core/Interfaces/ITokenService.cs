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
    string? ClientIpAddress { get; }

    long? UserId { get; }
}