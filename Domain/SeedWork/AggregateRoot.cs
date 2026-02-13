namespace Domain.SeedWork;

// <summary>
// Aggregate Root base interface
// for public use
// </summary>
public interface IAggregateRoot
{
    public IReadOnlyList<IDomainEvent> DomainEvents { get; }

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
    public abstract int Version { get; }
    public abstract int? StoredVersion { get; }

    private readonly List<BaseDomainEvent<TID>> _domainEvents = [];

    public IReadOnlyList<BaseDomainEvent<TID>> DomainEvents => _domainEvents.AsReadOnly();
    IReadOnlyList<IDomainEvent> IAggregateRoot.DomainEvents => DomainEvents;


    protected void AddDomainEvent(BaseDomainEvent<TID> eventItem) => _domainEvents.Add(eventItem);
    protected void RemoveDomainEvent(BaseDomainEvent<TID> eventItem) => _domainEvents.Remove(eventItem);
    public void ClearDomainEvents() => _domainEvents.Clear();

}


// <summary>
// Generic Aggregate Root Base Class
// with state for Domain Correctness
// and Id for public usage
// </summary>
public abstract class AggregateRoot<TID, TS> : AggregateRoot<TID>
where TID : AggregateRootId
where TS : notnull, AggregateRootState<TID>, IAggregateRootStateFactory<TS, TID>
{
    public TS State { get; private init; }

    public override TID Id => State.Id;
    public override int Version => State.Version;
    public int NextVersion => State.Version + 1;
    public override int? StoredVersion { get; } = null;

    // <summary>
    // history must be in ASC order by Version
    // </summary>
    protected AggregateRoot(IEnumerable<IDomainEvent>? history)
    {
        State = TS.CreateState(this);
        foreach (var e in history ?? [])
        {
            ((dynamic)State).Apply((dynamic)e); // hack to call Apply override
            StoredVersion = State.Version = e.Version;
        }
    }

    // <summary>
    // history must be in ASC order by Version
    // </summary>
    protected AggregateRoot(ref TS storedState)
    {
        State = storedState;
        StoredVersion = State.Version;
    }



    // <summary>
    // matches Version field to the appended events verson
    // </summary>
    protected void AppendEvent(IDomainEvent e)
    {
        AddDomainEvent((BaseDomainEvent<TID>)e);
        ((dynamic)State).Apply((dynamic)e); // hack to call Apply override
        State.Version = e.Version;
    }


#pragma warning disable CS8618 
    // <summary>
    // For entity Framework
    // </summary>
    protected AggregateRoot() { }
#pragma warning restore CS8618 
}

