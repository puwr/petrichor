namespace Petrichor.Shared.Domain.Common;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTime OccurredAtUtc { get; }
}