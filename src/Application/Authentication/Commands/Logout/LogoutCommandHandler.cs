using Application.Common.Interfaces.Services;
using Domain.Users;
using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Authentication.Commands.Logout;

public class LogoutCommandHandler(
    UserManager<User> userManager,
    ICookieService cookieService)
    : IRequestHandler<LogoutCommand, ErrorOr<Success>>
{
    public async ValueTask<ErrorOr<Success>> Handle(
        LogoutCommand command,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(command.RefreshToken))
        {
            var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == command.RefreshToken,
                cancellationToken: cancellationToken);

            if (user is not null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiresAtUtc = null;

                await userManager.UpdateAsync(user);
            }
        }

        cookieService.DeleteCookie("ACCESS_TOKEN");
        cookieService.DeleteCookie("REFRESH_TOKEN");

        return Result.Success;
    }
}
