using Petrichor.Shared.Domain.Common;

namespace Petrichor.Shared.Application.Common.Events;

public interface IDomainEventHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
