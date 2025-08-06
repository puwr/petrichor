namespace Petrichor.Shared.Domain.Common;

public interface IHasDomainEvents
{
    List<DomainEvent> PopDomainEvents();
}
