using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Petrichor.Shared.Application.Common.Events;
using Petrichor.Shared.Domain.Common;

namespace Petrichor.Shared.Infrastructure.Outbox;

public class DomainEventDispatcher<TDbContext>(
    IServiceProvider services,
    TDbContext dbContext)
    where TDbContext : IOutboxDbContext
{
    public async Task DispatchAsync<TDomainEvent>(
        TDomainEvent @event,
        CancellationToken cancellationToken = default)
        where TDomainEvent : IDomainEvent
    {
        var handlers = services.GetServices<IDomainEventHandler<TDomainEvent>>();

        if (!handlers.Any()) return;

        List<Task> tasks = [];

        foreach (var handler in handlers)
        {
            var outboxMessageConsumer = new OutboxMessageConsumer(@event.Id, handler.GetType().FullName!);

            if (await OutboxConsumerExistsAsync(outboxMessageConsumer)) continue;

            tasks.Add(Task.Run(async () =>
            {
                await handler.Handle(@event, cancellationToken);
                dbContext.OutboxMessageConsumers.Add(outboxMessageConsumer);
            }, cancellationToken));
        }

        await Task.WhenAll(tasks);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<bool> OutboxConsumerExistsAsync(OutboxMessageConsumer outboxMessageConsumer)
    {
        var exists = await dbContext.OutboxMessageConsumers
            .FirstOrDefaultAsync(im => im.OutboxMessageId == outboxMessageConsumer.OutboxMessageId
                && im.Name == outboxMessageConsumer.Name) is not null;

        return exists;
    }
}