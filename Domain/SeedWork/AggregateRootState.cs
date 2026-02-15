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


public abstract class AggregateRootState<TID> : IAggregateRootState
where TID : EntityId
{
    public AggregateRoot<TID>? Owner { get; }

    public TID Id { get; protected set; }
    public int Version { get; set; }

    IAggregateRoot IAggregateRootState.Owner => Owner!;


#pragma warning disable CS8618
    protected AggregateRootState(AggregateRoot<TID> owner) => Owner = owner;
#pragma warning restore CS8618

#pragma warning disable CS8618
    protected AggregateRootState() { }
#pragma warning restore CS8618
}

