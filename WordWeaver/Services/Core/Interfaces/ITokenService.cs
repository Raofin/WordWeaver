using WordWeaver.Data.Entity;
using WordWeaver.Dtos;

namespace WordWeaver.Services.Core.Interfaces
{
    public interface ITokenService
    {
        string ClientIpAddress { get; }

        (string token, DateTime expiresAt) GenerateAuthToken(User user);

        DecodedJwt DecodeJwt();
    }
}