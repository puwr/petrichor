namespace Petrichor.Shared.Application.Common.Events;

public class IntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; init; }

    public DateTime OccurredAtUtc { get; init; }

    protected IntegrationEvent()
    {
        Id = Guid.NewGuid();
        OccurredAtUtc = DateTime.UtcNow;
    }
}