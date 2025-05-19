using Application.Common.Interfaces;
using Domain.Users;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IUsersRepository usersRepository,
    IJwtTokenProvider jwtTokenProvider,
    UserManager<User> userManager) : IRequestHandler<RefreshTokenCommand, ErrorOr<Success>>
{
    private readonly IUsersRepository _usersRepository = usersRepository;
    private readonly IJwtTokenProvider _jwtTokenProvider = jwtTokenProvider;
    private readonly UserManager<User> _userManager = userManager;

    public async Task<ErrorOr<Success>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        if (command.RefreshToken is null)
        {
            return Error.Unauthorized(description: "Refresh token is missing.");
        }

        var user = await _usersRepository.GetUserByRefreshTokenAsync(command.RefreshToken);

        if (user is null)
        {
            return Error.Unauthorized(description: "Unable to retrieve user with provided refresh token.");
        }

        if (user.RefreshTokenExpiresAtUtc < DateTime.UtcNow)
        {
            return Error.Unauthorized(description: "Refresh token is expired.");
        }

        var (accessToken, accessTokenExpirationDateInUtc) = _jwtTokenProvider.GenerateAccessToken(user);
        var (refreshToken, refreshTokenExpirationDateInUtc) = _jwtTokenProvider.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAtUtc = refreshTokenExpirationDateInUtc;

        await _userManager.UpdateAsync(user);

        _jwtTokenProvider.WriteTokenAsHttpOnlyCookie(
            "ACCESS_TOKEN",
            accessToken,
            accessTokenExpirationDateInUtc);

        _jwtTokenProvider.WriteTokenAsHttpOnlyCookie(
            "REFRESH_TOKEN",
            refreshToken,
            refreshTokenExpirationDateInUtc
        );

        return Result.Success;
    }
}
