using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Petrichor.Services.Users.Features.Account.GetCurrentUser;

public static class GetCurrentUserQueryHandler
{
    public static GetCurrentUserResponse Handle(GetCurrentUserQuery query)
    {
        var id = query.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? throw new InvalidOperationException("User id claim is missing from JWT.");

        var email = query.User.FindFirstValue(JwtRegisteredClaimNames.Email)
            ?? throw new InvalidOperationException("User email claim is missing from JWT.");;

        var userName = query.User.FindFirstValue(JwtRegisteredClaimNames.UniqueName)
            ?? throw new InvalidOperationException("User name claim is missing from JWT.");

        var roles = query.User.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        return new GetCurrentUserResponse(
            id,
            email,
            userName,
            roles ?? []
        );
    }
}
