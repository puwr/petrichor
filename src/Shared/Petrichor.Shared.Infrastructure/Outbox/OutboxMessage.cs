using Newtonsoft.Json;
using Petrichor.Shared.Application.Common.Events;
using Petrichor.Shared.Domain.Common;
using Petrichor.Shared.Infrastructure.Serialization;

namespace Petrichor.Shared.Infrastructure.Outbox;

public class OutboxMessage
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string AssemblyQualifiedName { get; init; }
    public OutboxEventType Type { get; init; }
    public string Content { get; init; }
    public DateTime OccurredAtUtc { get; init; }
    public DateTime? ProcessedAtUtc { get; set; }
    public string? Error { get; set; }

    public static OutboxMessage From(DomainEvent @event)
        => new OutboxMessage
        {
            Id = @event.Id,
            Name = @event.GetType().Name,
            AssemblyQualifiedName = @event.GetType().AssemblyQualifiedName!,
            Type = OutboxEventType.Domain,
            Content = JsonConvert.SerializeObject(@event, SerializerSettings.Events),
            OccurredAtUtc = @event.OccurredAtUtc
        };

    public static OutboxMessage From(IntegrationEvent @event)
        => new OutboxMessage
        {
            Id = @event.Id,
            Name = @event.GetType().Name,
            AssemblyQualifiedName = @event.GetType().AssemblyQualifiedName!,
            Type = OutboxEventType.Integration,
            Content = JsonConvert.SerializeObject(@event, SerializerSettings.Events),
            OccurredAtUtc = @event.OccurredAtUtc
        };
}

public enum OutboxEventType { Domain, Integration }