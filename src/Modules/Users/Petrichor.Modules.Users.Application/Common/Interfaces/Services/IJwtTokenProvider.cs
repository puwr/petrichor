using Petrichor.Modules.Users.Application.Common.Interfaces.Services.Authentication;
using Petrichor.Modules.Users.Domain.Users;

namespace Petrichor.Modules.Users.Application.Common.Interfaces.Services;

public interface IJwtTokenProvider
{
    TokenResult GenerateAccessToken(User user);

    TokenResult GenerateRefreshToken();
}