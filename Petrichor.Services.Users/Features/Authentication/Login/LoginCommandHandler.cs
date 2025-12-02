using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Petrichor.Services.Users.Common.Domain;
using Petrichor.Services.Users.Common.Services;

namespace Petrichor.Services.Users.Features.Authentication.Login;

public static class LoginCommandHandler
{
    public static async Task<ErrorOr<Success>> Handle(
        LoginCommand request,
        UserManager<User> userManager,
        IJwtTokenProvider jwtTokenProvider,
        ICookieService cookieService)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Error.Validation(description: "Invalid credentials.");
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