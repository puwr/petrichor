using MediatR;

namespace Petrichor.Shared.Domain.Common;

public interface IDomainEvent : INotification
{
    Guid Id { get; }
    DateTime OccurredAtUtc { get; }
}