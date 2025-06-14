using Domain.Users;

namespace Application.Common.Interfaces.Services.Authentication;

public interface IJwtTokenProvider
{
    TokenResult GenerateAccessToken(User user);

    TokenResult GenerateRefreshToken();
}