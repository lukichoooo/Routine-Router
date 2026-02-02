namespace Domain.SeedWork;

// <summary>
// Aggregate Root base interface
// for public use
// </summary>
public interface IAggregateRoot
{
    public IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    public void ClearDomainEvents();

    public int Version { get; }
    public int? StoredVersion { get; }
}


// <summary>
// Generic Aggregate Root Base Class
// with Id for public usage
// </summary>
public abstract class AggregateRoot<TID> : IAggregateRoot
where TID : AggregateRootId
{
    public abstract TID Id { get; }

    private readonly List<BaseDomainEvent<TID>> _domainEvents = [];

    public IReadOnlyCollection<BaseDomainEvent<TID>> DomainEvents => _domainEvents.AsReadOnly();
    IReadOnlyCollection<IDomainEvent> IAggregateRoot.DomainEvents => DomainEvents;

    protected void AddDomainEvent(BaseDomainEvent<TID> eventItem) => _domainEvents.Add(eventItem);
    protected void RemoveDomainEvent(BaseDomainEvent<TID> eventItem) => _domainEvents.Remove(eventItem);
    public void ClearDomainEvents() => _domainEvents.Clear();

    public int Version { get; protected set; } = 0;
    public int NextVersion => Version + 1;
    public int? StoredVersion { get; protected set; } = null;
}


// <summary>
// Generic Aggregate Root Base Class
// with state for Domain Correctness
// and Id for public usage
// </summary>
public abstract class AggregateRoot<TID, TS> : AggregateRoot<TID>
where TID : AggregateRootId
where TS : notnull, IState<TID>, new()
{
    public override TID Id => State.Id;
    protected TS State { get; init; }

    // <summary>
    // history must be in ASC order by Version
    // </summary>
    protected AggregateRoot(IEnumerable<IDomainEvent>? history)
    {
        State = new TS();
        foreach (var e in history ?? [])
        {
            ((dynamic)State).Apply((dynamic)e); // hack to call Apply override
            StoredVersion = Version = e.Version;
        }
    }

    // <summary>
    // matches Version field to the appended events verson
    // </summary>
    protected void AppendEvent(IDomainEvent e)
    {
        AddDomainEvent((BaseDomainEvent<TID>)e);
        ((dynamic)State).Apply((dynamic)e); // hack to call Apply override
        Version = e.Version;
    }
}

