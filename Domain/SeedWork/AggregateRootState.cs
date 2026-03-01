namespace Domain.SeedWork;


public interface IAggregateRootStateFactory<TState, TID>
    where TState : AggregateRootState<TID>
    where TID : EntityId
{
    static abstract TState CreateState(AggregateRoot<TID> owner);
}


public interface IAggregateRootState
{
    public IAggregateRoot? Owner { get; }
}


public abstract class AggregateRootState<TId> : IAggregateRootState
where TId : EntityId
{
    public AggregateRoot<TId>? Owner { get; }

    public TId Id { get; protected set; }
    public int Version { get; set; }

    IAggregateRoot IAggregateRootState.Owner => Owner!;


#pragma warning disable CS8618
    protected AggregateRootState(AggregateRoot<TId> owner) => Owner = owner;
    protected AggregateRootState() { }
#pragma warning restore CS8618
}

