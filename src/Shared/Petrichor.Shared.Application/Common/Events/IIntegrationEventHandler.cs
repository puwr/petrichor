namespace Petrichor.Shared.Application.Common.Events;

public interface IIntegrationEventHandler<TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
{
    Task Handle(
        TIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default);
}