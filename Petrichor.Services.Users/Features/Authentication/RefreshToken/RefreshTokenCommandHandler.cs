using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Users.Common.Domain;
using Petrichor.Services.Users.Common.Services;

namespace Petrichor.Services.Users.Features.Authentication.RefreshToken;

public static class RefreshTokenCommandHandler
{
    public static async Task<ErrorOr<Success>> Handle(
        RefreshTokenCommand command,
        IJwtTokenProvider jwtTokenProvider,
        UserManager<User> userManager,
        ICookieService cookieService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.RefreshToken))
        {
            return Error.Unauthorized(description: "Refresh token is missing.");
        }

        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == command.RefreshToken,
                cancellationToken: cancellationToken);

        if (user is null)
        {
            return Error
                .Unauthorized(description:
                    "Unable to retrieve user with provided refresh token.");
        }

        if (user.RefreshTokenExpiresAtUtc < DateTime.UtcNow)
        {
            return Error.Unauthorized(description: "Refresh token is expired.");
        }

        var roles = await userManager.GetRolesAsync(user);

        var accessTokenResult = jwtTokenProvider.GenerateAccessToken(user, roles);

        var refreshTokenResult = jwtTokenProvider.GenerateRefreshToken();

        user.RefreshToken = refreshTokenResult.Token;
        user.RefreshTokenExpiresAtUtc = refreshTokenResult.ExpiresAt;

        await userManager.UpdateAsync(user);

        cookieService.WriteCookie(
            "ACCESS_TOKEN",
            accessTokenResult.Token,
            accessTokenResult.ExpiresAt);

        cookieService.WriteCookie(
            "REFRESH_TOKEN",
            refreshTokenResult.Token,
            refreshTokenResult.ExpiresAt);

        return Result.Success;
    }
}