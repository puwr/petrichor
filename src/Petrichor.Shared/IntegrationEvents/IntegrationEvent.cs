namespace Petrichor.Shared.Events;

public class IntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; init; }

    public DateTime OccurredAtUtc { get; init; }

    protected IntegrationEvent()
    {
        Id = Guid.CreateVersion7();
        OccurredAtUtc = DateTime.UtcNow;
    }
}