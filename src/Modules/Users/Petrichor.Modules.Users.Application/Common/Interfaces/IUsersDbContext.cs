using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Petrichor.Modules.Users.Domain.Users;
using Petrichor.Shared.Infrastructure.Outbox;

namespace Petrichor.Modules.Users.Application.Common.Interfaces;

public interface IUsersDbContext
{
    DbSet<User> Users { get; }
    DbSet<IdentityRole<Guid>> Roles { get; }
    DbSet<IdentityUserRole<Guid>> UserRoles { get; }

    DbSet<OutboxMessage> OutboxMessages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
