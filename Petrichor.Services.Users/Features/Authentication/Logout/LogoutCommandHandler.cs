using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Users.Common.Domain;
using Petrichor.Services.Users.Common.Services;

namespace Petrichor.Services.Users.Features.Authentication.Logout;

public static class LogoutCommandHandler
{
    public static async Task<ErrorOr<Success>> Handle(
        LogoutCommand command,
        UserManager<User> userManager,
        ICookieService cookieService,
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