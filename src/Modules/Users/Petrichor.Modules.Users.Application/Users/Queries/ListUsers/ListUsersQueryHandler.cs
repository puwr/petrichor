using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Petrichor.Modules.Users.Application.Common.Interfaces;
using Petrichor.Modules.Users.Contracts.Users;
using Petrichor.Modules.Users.Domain.Users;
using Petrichor.Shared.Application.Extensions;
using Petrichor.Shared.Contracts.Pagination;

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
            .OrderByDescending(u => u.RegisteredAtUtc)
            .Select(u => new ListUsersResponse(
                    u.Id,
                    u.Email!,
                    u.UserName!,
                    dbContext.Roles
                        .Where(r => dbContext.UserRoles
                            .Any(ur => ur.UserId == u.Id && ur.RoleId == r.Id))
                        .Select(r => r.Name ?? string.Empty)
                        .ToList()))
            .ToPagedResponseAsync(request.Pagination, cancellationToken);

        return users;
    }
}