namespace Petrichor.Shared.Domain.Common;

public interface IHasDomainEvents
{
    List<IDomainEvent> PopDomainEvents();
}
