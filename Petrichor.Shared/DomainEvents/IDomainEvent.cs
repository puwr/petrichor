namespace Petrichor.Shared.DomainEvents;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTime OccurredAtUtc { get; }
}