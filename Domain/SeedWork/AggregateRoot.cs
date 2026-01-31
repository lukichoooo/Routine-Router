namespace Domain.SeedWork;


public abstract class AggregateRoot<TAggregateId>
{
    private readonly List<IDomainEvent<TAggregateId>> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent<TAggregateId>> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent<TAggregateId> eventItem) => _domainEvents.Add(eventItem);
    public void RemoveDomainEvent(IDomainEvent<TAggregateId> eventItem) => _domainEvents.Remove(eventItem);
    public void ClearDomainEvents() => _domainEvents.Clear();

    public int Version { get; internal set; } = 0;
    public int NextVersion => Version + 1;
    
    public abstract TAggregateId Id { get; }
}

