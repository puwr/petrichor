using Microsoft.EntityFrameworkCore;

namespace Petrichor.Shared.Infrastructure.Outbox;

public interface IOutboxDbContext
{
    DbSet<OutboxMessage> OutboxMessages { get; }
    DbSet<OutboxMessageConsumer> OutboxMessageConsumers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}