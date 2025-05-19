using Application.Common.Interfaces;
using Domain.Users;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Authentication.Queries.Login;

public class LoginQueryHandler(
    UserManager<User> userManager,
    IJwtTokenProvider jwtTokenProvider) : IRequestHandler<LoginQuery, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Error.Validation(
                code: "Authentication.InvalidCredentials",
                description: "Invalid credentials.");
        }

        var (accessToken, accessTokenExpirationDateInUtc) = jwtTokenProvider.GenerateAccessToken(user);
        var (refreshToken, refreshTokenExpirationDateInUtc) = jwtTokenProvider.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAtUtc = refreshTokenExpirationDateInUtc;

        await userManager.UpdateAsync(user);

        jwtTokenProvider.WriteTokenAsHttpOnlyCookie(
            "ACCESS_TOKEN",
            accessToken,
            accessTokenExpirationDateInUtc);

        jwtTokenProvider.WriteTokenAsHttpOnlyCookie(
            "REFRESH_TOKEN",
            refreshToken,
            refreshTokenExpirationDateInUtc
        );

        return Result.Success;
    }
}
