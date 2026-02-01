namespace Domain.SeedWork;


public abstract class AggregateRoot<TAggregateRootId>
where TAggregateRootId : AggregateRootId
{
    public abstract TAggregateRootId Id { get; }

    private readonly List<IDomainEvent<TAggregateRootId>> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent<TAggregateRootId>> DomainEvents => _domainEvents.AsReadOnly();

    internal void AddDomainEvent(IDomainEvent<TAggregateRootId> eventItem) => _domainEvents.Add(eventItem);
    internal void RemoveDomainEvent(IDomainEvent<TAggregateRootId> eventItem) => _domainEvents.Remove(eventItem);
    public void ClearDomainEvents() => _domainEvents.Clear();

    public int Version { get; internal set; } = 0;
    public int NextVersion => Version + 1;
}

