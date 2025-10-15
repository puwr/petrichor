using Petrichor.Services.Users.Common.Domain;

namespace Petrichor.Services.Users.Common.Services;

public interface IJwtTokenProvider
{
    TokenResult GenerateAccessToken(User user, IList<string> roles);

    TokenResult GenerateRefreshToken();
}