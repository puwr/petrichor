using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Users.Common.Domain;
using Petrichor.Services.Users.Common.Persistence;
using Petrichor.Services.Users.Common.Services;

namespace Petrichor.Services.Users.Features.Authentication.RefreshToken;

public static class RefreshTokenCommandHandler
{
    public static async Task<ErrorOr<Success>> Handle(
        RefreshTokenCommand command,
        UsersDbContext dbContext,
        IJwtTokenProvider jwtTokenProvider,
        UserManager<User> userManager,
        ICookieService cookieService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.RefreshToken))
        {
            return Error.Unauthorized(description: "Refresh token is missing.");
        }

        var refreshToken = await dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == command.RefreshToken, cancellationToken);

        if (refreshToken is null)
        {
            return Error
                .Unauthorized(description:
                    "Unable to retrieve user with provided refresh token.");
        }

        if (refreshToken.ExpiresAtUtc < DateTime.UtcNow)
        {
            return Error.Unauthorized(description: "Refresh token is expired.");
        }

        var roles = await userManager.GetRolesAsync(refreshToken.User);
        var accessTokenResult = jwtTokenProvider.GenerateAccessToken(refreshToken.User, roles);
        var refreshTokenResult = jwtTokenProvider.GenerateRefreshToken();

        refreshToken.Renew(refreshTokenResult);

        await dbContext.SaveChangesAsync(cancellationToken);

        cookieService.WriteCookie(
            "ACCESS_TOKEN",
            accessTokenResult.Token,
            accessTokenResult.ExpiresAtUtc);

        cookieService.WriteCookie(
            "REFRESH_TOKEN",
            refreshTokenResult.Token,
            refreshTokenResult.ExpiresAtUtc);

        return Result.Success;
    }
}