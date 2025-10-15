namespace Petrichor.Shared.Events;

public interface IIntegrationEventHandler<TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
{
    Task Handle(
        TIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default);
}