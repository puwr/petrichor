namespace Petrichor.Shared.Application.Common.Events;

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime OccurredAtUtc { get; }
}