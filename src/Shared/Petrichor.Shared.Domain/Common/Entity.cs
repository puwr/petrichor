namespace Petrichor.Shared.Domain.Common;

public abstract class Entity : IHasDomainEvents
{
    protected readonly List<DomainEvent> _domainEvents = [];

    public List<DomainEvent> PopDomainEvents()
    {
        var copy = _domainEvents.ToList();

        _domainEvents.Clear();

        return copy;
    }

    protected Entity() {}
}