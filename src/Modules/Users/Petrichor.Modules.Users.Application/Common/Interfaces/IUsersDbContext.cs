using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Petrichor.Modules.Users.Domain.Users;
using Petrichor.Shared.Inbox;
using Petrichor.Shared.Outbox;

namespace Petrichor.Modules.Users.Application.Common.Interfaces;

public interface IUsersDbContext : IInboxDbContext, IOutboxDbContext
{
    DbSet<User> Users { get; }
    DbSet<IdentityRole<Guid>> Roles { get; }
    DbSet<IdentityUserRole<Guid>> UserRoles { get; }

    new Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}