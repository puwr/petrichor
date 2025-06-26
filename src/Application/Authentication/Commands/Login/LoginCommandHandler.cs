using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Authentication;
using Domain.Users;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Authentication.Commands.Login;

public class LoginCommandHandler(
    UserManager<User> userManager,
    IJwtTokenProvider jwtTokenProvider,
    ICookieService cookieService)
    : IRequestHandler<LoginCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Error.Validation("Invalid credentials.");
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