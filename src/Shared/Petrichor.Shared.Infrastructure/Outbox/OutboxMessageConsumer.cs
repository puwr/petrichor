namespace Petrichor.Shared.Infrastructure.Outbox;

public class OutboxMessageConsumer(Guid outboxMessageId, string name)
{
    public Guid OutboxMessageId { get; init; } = outboxMessageId;
    public string Name { get; init; } = name;
}