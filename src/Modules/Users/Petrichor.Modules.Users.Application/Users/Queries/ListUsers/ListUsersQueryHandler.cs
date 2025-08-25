using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Petrichor.Modules.Users.Application.Common.Interfaces;
using Petrichor.Modules.Users.Contracts.Users;
using Petrichor.Shared.Pagination;
using Petrichor.Shared.Extensions;

namespace Petrichor.Modules.Users.Application.Users.Queries.ListUsers;

public class ListUsersQueryHandler(IUsersDbContext dbContext)
    : IRequestHandler<ListUsersQuery, ErrorOr<PagedResponse<ListUsersResponse>>>
{
    public async Task<ErrorOr<PagedResponse<ListUsersResponse>>> Handle(
        ListUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = await dbContext.Users
            .AsNoTracking()
            .IgnoreQueryFilters()
            .OrderByDescending(u => u.RegisteredAtUtc)
            .Select(u => new ListUsersResponse(
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