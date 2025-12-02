using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Petrichor.Shared.Pagination;
using Petrichor.Shared.Extensions;
using Petrichor.Services.Users.Common.Persistence;

namespace Petrichor.Services.Users.Features.Users.GetUsers;

public static class GetUsersQueryHandler
{
    public static async Task<ErrorOr<PagedResponse<GetUsersResponse>>> Handle(
        GetUsersQuery request,
        UsersDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var users = await dbContext.Users
            .AsNoTracking()
            .IgnoreQueryFilters()
            .OrderByDescending(u => u.RegisteredAtUtc)
            .Select(u => new GetUsersResponse(
                    u.Id,
                    u.Email!,
                    u.UserName!,
                    dbContext.Roles
                        .Where(r => dbContext.UserRoles
                            .Any(ur => ur.UserId == u.Id && ur.RoleId == r.Id))
                        .Select(r => r.Name ?? string.Empty)
                        .ToList(),
                    u.IsDeleted))
            .ToPagedResponseAsync(request.Pagination, cancellationToken);

        return users;
    }
}