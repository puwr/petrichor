using Microsoft.EntityFrameworkCore;
using Petrichor.Services.Users.Common.Persistence;
using Petrichor.Shared.Extensions;
using Petrichor.Shared.Pagination;

namespace Petrichor.Services.Users.Features.Users.GetUsers;

public static class GetUsersQueryHandler
{
    public static async Task<PagedResponse<GetUsersResponse>> Handle(
        GetUsersQuery query,
        UsersDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var response = await dbContext.Users
            .AsNoTracking()
            .OrderByDescending(u => u.RegisteredAtUtc)
            .Select(u => new GetUsersResponse(
                    u.Id,
                    u.Email!,
                    u.UserName!,
                    u.RegisteredAtUtc,
                    dbContext.Roles
                        .Where(r => dbContext.UserRoles
                            .Any(ur => ur.UserId == u.Id && ur.RoleId == r.Id))
                        .Select(r => r.Name ?? string.Empty)
                        .ToList(),
                    u.IsDeleted))
            .ToPagedResponseAsync(query.Pagination, cancellationToken);

        return response;
    }
}