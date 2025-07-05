using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Authentication;
using Domain.Users;
using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IJwtTokenProvider jwtTokenProvider,
    UserManager<User> userManager,
    ICookieService cookieService)
    : IRequestHandler<RefreshTokenCommand, ErrorOr<Success>>
{
    public async ValueTask<ErrorOr<Success>> Handle(
        RefreshTokenCommand command,
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

        var accessTokenResult = jwtTokenProvider.GenerateAccessToken(user);

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