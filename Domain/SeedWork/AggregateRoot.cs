namespace Domain.SeedWork;

public interface IEntityFactory<TEntity>
    where TEntity : IAggregateRoot
{
    static abstract TEntity Create(IEnumerable<IDomainEvent>? history);
}

public interface IEntityFactory<TEntity, TId, TState> : IEntityFactory<TEntity>
    where TId : EntityId
    where TState : AggregateRootState<TId>, IAggregateRootStateFactory<TState, TId>
    where TEntity : AggregateRoot<TId, TState>
{
    static abstract TEntity Create(TState storedState);
}


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
public abstract class AggregateRoot<TId> : IAggregateRoot
where TId : EntityId
{
    public abstract TId Id { get; }
    public abstract int Version { get; }
    public abstract int? StoredVersion { get; }

    private readonly List<BaseDomainEvent<TId>> _domainEvents = [];

    public IReadOnlyList<BaseDomainEvent<TId>> DomainEvents => _domainEvents.AsReadOnly();
    IReadOnlyList<IDomainEvent> IAggregateRoot.DomainEvents => DomainEvents;


    protected void AddDomainEvent(BaseDomainEvent<TId> eventItem) => _domainEvents.Add(eventItem);
    protected void RemoveDomainEvent(BaseDomainEvent<TId> eventItem) => _domainEvents.Remove(eventItem);
    public void ClearDomainEvents() => _domainEvents.Clear();

}


// <summary>
// Generic Aggregate Root Base Class
// with state for Domain Correctness and Id for public usage
// </summary>
public abstract class AggregateRoot<TId, TState> : AggregateRoot<TId>
where TId : EntityId
where TState : notnull, AggregateRootState<TId>, IAggregateRootStateFactory<TState, TId>
{
    public TState State { get; private init; }

    public override TId Id => State.Id;
    public override int Version => State.Version;
    public int NextVersion => State.Version + 1;
    public override int? StoredVersion { get; } = null;

    // <summary>
    // history must be in ASC order by Version
    // </summary>
    protected AggregateRoot(IEnumerable<IDomainEvent>? history)
    {
        State = TState.CreateState(this);
        foreach (var e in history ?? [])
        {
            ((dynamic)State).Apply((dynamic)e); // hack to call Apply override
            StoredVersion = State.Version = e.Version;
        }
    }

    // <summary>
    // history must be in ASC order by Version
    // </summary>
    protected AggregateRoot(TState storedState)
    {
        State = storedState;
        StoredVersion = State.Version;
    }


    // <summary>
    // matches Version field to the appended events verson
    // </summary>
    protected void AppendEvent(IDomainEvent e)
    {
        AddDomainEvent((BaseDomainEvent<TId>)e);
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

