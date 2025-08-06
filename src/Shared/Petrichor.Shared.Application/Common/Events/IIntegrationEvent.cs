using MediatR;

namespace Petrichor.Shared.Application.Common.Events;

public interface IIntegrationEvent : INotification
{
    Guid Id { get; }
    DateTime OccurredAtUtc { get; }
}