using Petrichor.Shared.Application.Common.Events;

namespace Petrichor.Shared.Infrastructure.Outbox;

public class IntegrationEventPublisher<TIDbContext>(TIDbContext dbContext)
    where TIDbContext : IOutboxDbContext
{
    public void Publish<TIntegrationEvent>(
        TIntegrationEvent @event)
        where TIntegrationEvent : IntegrationEvent
    {
        var outboxMessage = OutboxMessage.From(@event);

        dbContext.OutboxMessages.Add(outboxMessage);
    }
}