using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Users.Common.Persistence;
using Petrichor.Services.Users.Common.Services;

namespace Petrichor.Services.Users.Features.Authentication.Logout;

public static class LogoutCommandHandler
{
    public static async Task<ErrorOr<Success>> Handle(
        LogoutCommand command,
        UsersDbContext dbContext,
        ICookieService cookieService,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(command.RefreshToken))
        {
            await dbContext.RefreshTokens
                .Where(rt => rt.Token == command.RefreshToken)
                .ExecuteDeleteAsync(cancellationToken);
        }

        cookieService.DeleteCookie("ACCESS_TOKEN");
        cookieService.DeleteCookie("REFRESH_TOKEN");

        return Result.Success;
    }
}