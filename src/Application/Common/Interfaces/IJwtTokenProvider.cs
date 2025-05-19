using Domain.Users;

namespace Application.Common.Interfaces;

public interface IJwtTokenProvider
{
    (string accessToken, DateTime accessTokenExpirationDateInUtc) GenerateAccessToken(User user);
    (string refreshToken, DateTime refreshTokenExpirationDateInUtc) GenerateRefreshToken();
    void WriteTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiresAt);
}