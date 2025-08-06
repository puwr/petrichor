
namespace Petrichor.Shared.Domain.Common;

public class DomainEvent : IDomainEvent
{
    public Guid Id { get; init; }

    public DateTime OccurredAtUtc { get; init; }

    protected DomainEvent()
    {
        Id = Guid.NewGuid();
        OccurredAtUtc = DateTime.UtcNow;
    }
}