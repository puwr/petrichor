using Microsoft.EntityFrameworkCore.Diagnostics;
using Petrichor.Shared.Domain.Common;
using Petrichor.Shared.Infrastructure.Serialization;

namespace Petrichor.Shared.Infrastructure.Outbox;

public class InsertOutboxMessagesInterceptor : SaveChangesInterceptor
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
                .Select(domainEvent => new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    Type = domainEvent.GetType().Name,
                    Content = DomainEventsSerializer.Serialize(domainEvent),
                    OccurredAtUtc = DateTime.UtcNow
                }).ToList();

            eventData.Context.Set<OutboxMessage>().AddRange(outboxMessages);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}