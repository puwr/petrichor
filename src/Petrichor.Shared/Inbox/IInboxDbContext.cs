using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Petrichor.Shared.Inbox;

public interface IInboxDbContext
{
    DbSet<InboxMessage> InboxMessages { get; }
    DbSet<InboxMessageConsumer> InboxMessageConsumers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}