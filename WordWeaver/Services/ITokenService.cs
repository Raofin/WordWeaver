using WordWeaver.Data.Entity;

namespace WordWeaver.Services
{
    public interface ITokenService
    {
        string ClientIpAddress { get; }

        (string token, DateTime expiresAt) GenerateAuthToken(User user);

        DecodedJwt DecodeJwt();
    }
}