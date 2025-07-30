namespace Petrichor.Shared.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; private set; }

    protected readonly List<IDomainEvent> _domainEvents = [];

    protected Entity(Guid id) => Id = id;

    public List<IDomainEvent> PopDomainEvents()
    {
        var copy = _domainEvents.ToList();

        _domainEvents.Clear();

        return copy;
    }

    protected Entity() {}
}