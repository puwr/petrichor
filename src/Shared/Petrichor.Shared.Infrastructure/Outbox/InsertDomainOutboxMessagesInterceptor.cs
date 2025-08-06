using Microsoft.EntityFrameworkCore.Diagnostics;
using Petrichor.Shared.Domain.Common;

namespace Petrichor.Shared.Infrastructure.Outbox;

public class InsertDomainOutboxMessagesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            var outboxMessages = eventData.Context.ChangeTracker
                .Entries<IHasDomainEvents>()
                .Select(entry => entry.Entity.PopDomainEvents())
                .SelectMany(e => e)
                .Select(OutboxMessage.From)
                .ToList();

            eventData.Context.Set<OutboxMessage>().AddRange(outboxMessages);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}