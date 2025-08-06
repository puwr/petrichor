using Microsoft.EntityFrameworkCore;

namespace Petrichor.Shared.Infrastructure.Inbox;

public interface IInboxDbContext
{
    DbSet<InboxMessage> InboxMessages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}