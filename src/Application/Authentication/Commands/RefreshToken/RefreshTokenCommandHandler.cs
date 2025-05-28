using Application.Common.Interfaces.Services.Authentication;
using Domain.Users;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IJwtTokenProvider jwtTokenProvider,
    UserManager<User> userManager)
    : IRequestHandler<RefreshTokenCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        if (command.RefreshToken is null)
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

        jwtTokenProvider.WriteTokenAsHttpOnlyCookie(
            "ACCESS_TOKEN",
            accessTokenResult.Token,
            accessTokenResult.ExpiresAt);

        jwtTokenProvider.WriteTokenAsHttpOnlyCookie(
            "REFRESH_TOKEN",
            refreshTokenResult.Token,
            refreshTokenResult.ExpiresAt
        );

        return Result.Success;
    }
}