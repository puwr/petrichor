using MassTransit;
using Newtonsoft.Json;
using Petrichor.Modules.Users.Infrastructure.Persistence;
using Petrichor.Shared.Application.Common.Events;
using Petrichor.Shared.Infrastructure.Inbox;
using Petrichor.Shared.Infrastructure.Serialization;

namespace Petrichor.Modules.Users.Infrastructure.Inbox;

public class IntegrationEventConsumer<TIntegrationEvent>(UsersDbContext dbContext)
    : IConsumer<TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent
{
    public async Task Consume(ConsumeContext<TIntegrationEvent> context)
    {
        TIntegrationEvent integrationEvent = context.Message;

        var inboxMessage = new InboxMessage
        {
            Id = integrationEvent.Id,
            Name = integrationEvent.GetType().Name,
            Content = JsonConvert.SerializeObject(integrationEvent, SerializerSettings.Events)!,
            OccurredAtUtc = DateTime.UtcNow
        };

        dbContext.InboxMessages.Add(inboxMessage);
        await dbContext.SaveChangesAsync();
    }
}