using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Petrichor.Services.Users.Common.Domain;
using Petrichor.Services.Users.Common.Persistence;
using Petrichor.Services.Users.Common.Services;

namespace Petrichor.Services.Users.Features.Authentication.Login;

public static class LoginCommandHandler
{
    public static async Task<ErrorOr<Success>> Handle(
        LoginCommand request,
        UsersDbContext dbContext,
        UserManager<User> userManager,
        IJwtTokenProvider jwtTokenProvider,
        ICookieService cookieService,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Error.Validation(description: "Invalid credentials.");
        }

        var roles = await userManager.GetRolesAsync(user);

        var accessTokenResult = jwtTokenProvider.GenerateAccessToken(user, roles);
        var refreshTokenResult = jwtTokenProvider.GenerateRefreshToken();

        var newRefreshToken = Common.Domain.RefreshToken.Create(refreshTokenResult, user.Id);
        dbContext.RefreshTokens.Add(newRefreshToken);

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