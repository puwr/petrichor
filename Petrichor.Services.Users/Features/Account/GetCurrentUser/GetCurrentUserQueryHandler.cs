using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ErrorOr;

namespace Petrichor.Services.Users.Features.Account.GetCurrentUser;

public static class GetCurrentUserInfoQueryHandler
{
    public static Task<ErrorOr<GetCurrentUserResponse>> Handle(GetCurrentUserQuery request)
    {
        var user = request.User;

        var id = user
            .FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrEmpty(id))
        {
            return Task.FromResult(Error
                .Failure("User id claim is missing.")
                .ToErrorOr<GetCurrentUserResponse>());
        }

        var email = user
            .FindFirstValue(JwtRegisteredClaimNames.Email);

        if (string.IsNullOrEmpty(email))
        {
            return Task.FromResult(Error
                .Failure("User email claim is missing.")
                .ToErrorOr<GetCurrentUserResponse>());
        }

        var userName = user
            .FindFirstValue(JwtRegisteredClaimNames.UniqueName);

        if (string.IsNullOrEmpty(userName))
        {
            return Task.FromResult(Error
                .Failure("User name claim is missing.")
                .ToErrorOr<GetCurrentUserResponse>());
        }

        var roles = user.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        return Task.FromResult(new GetCurrentUserResponse(
            id,
            email,
            userName,
            roles ?? []
        ).ToErrorOr());
    }
}
