using MassTransit;
using Newtonsoft.Json;
using Petrichor.Services.Comments.Api.Common.Persistence;
using Petrichor.Shared.Events;
using Petrichor.Shared.Inbox;
using Petrichor.Shared.Serialization;

namespace Petrichor.Services.Comments.Api.Common.Inbox;

public class IntegrationEventConsumer<TIntegrationEvent>(CommentsDbContext dbContext)
    : IConsumer<TIntegrationEvent>
    where TIntegrationEvent : class, IIntegrationEvent
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