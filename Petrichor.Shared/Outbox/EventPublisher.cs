using Petrichor.Shared.DomainEvents;
using Petrichor.Shared.Events;

namespace Petrichor.Shared.Outbox;

public class EventPublisher<TDbContext>(TDbContext dbContext)
    where TDbContext : IOutboxDbContext
{
    public void Publish(DomainEvent @event)
    {
        var outboxMessage = OutboxMessage.From(@event);

        dbContext.OutboxMessages.Add(outboxMessage);
    }

    public void Publish(IntegrationEvent @event)
    {
        var outboxMessage = OutboxMessage.From(@event);

        dbContext.OutboxMessages.Add(outboxMessage);
    }
}