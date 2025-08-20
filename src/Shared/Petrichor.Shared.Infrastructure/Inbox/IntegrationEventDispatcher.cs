using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Petrichor.Shared.Application.Common.Events;

namespace Petrichor.Shared.Infrastructure.Inbox;

public class IntegrationEventDispatcher<TDbContext>(
    IServiceProvider services,
    TDbContext dbContext)
    where TDbContext : IInboxDbContext
{
    public async Task DispatchAsync<TIntegrationEvent>(
        TIntegrationEvent @event,
        CancellationToken cancellationToken = default)
        where TIntegrationEvent : IIntegrationEvent
    {
        var handlers = services.GetServices<IIntegrationEventHandler<TIntegrationEvent>>();

        if (!handlers.Any()) return;

        List<Task> tasks = [];

        foreach (var handler in handlers)
        {
            var inboxMessageConsumer = new InboxMessageConsumer(@event.Id, handler.GetType().FullName!);

            if (await InboxConsumerExistsAsync(inboxMessageConsumer)) continue;

            tasks.Add(Task.Run(async () =>
            {
                await handler.Handle(@event, cancellationToken);
                dbContext.InboxMessageConsumers.Add(inboxMessageConsumer);
            }, cancellationToken));
        }

        await Task.WhenAll(tasks);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<bool> InboxConsumerExistsAsync(InboxMessageConsumer inboxMessageConsumer)
    {
        var exists = await dbContext.InboxMessageConsumers
            .FirstOrDefaultAsync(im => im.InboxMessageId == inboxMessageConsumer.InboxMessageId
                && im.Name == inboxMessageConsumer.Name) is not null;

        return exists;
    }
}