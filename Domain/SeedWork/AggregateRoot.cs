namespace Domain.SeedWork;


// <summary>
// Aggregate Root Base Class
// Meant to be disposed after used by handlers (for transient use)
// </summary>
public abstract class AggregateRoot<TID>
where TID : AggregateRootId
{
    public abstract TID Id { get; }

    private readonly List<IDomainEvent<TID>> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent<TID>> DomainEvents => _domainEvents.AsReadOnly();

    internal void AddDomainEvent(IDomainEvent<TID> eventItem) => _domainEvents.Add(eventItem);
    internal void RemoveDomainEvent(IDomainEvent<TID> eventItem) => _domainEvents.Remove(eventItem);

    public void ClearDomainEvents() => _domainEvents.Clear();

    public int Version { get; internal set; } = 0;
    public int NextVersion => Version + 1;
}

