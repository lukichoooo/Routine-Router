namespace Domain.SeedWork;


public interface IAggregateRootStateFactory<TState, TID>
    where TState : AggregateRootState<TID>
    where TID : AggregateRootId
{
    static abstract TState CreateState(AggregateRoot<TID> owner);
}

public abstract class AggregateRootState<TID>
where TID : AggregateRootId
{
    public AggregateRoot<TID> Owner { get; }

    public TID Id { get; protected set; }
    public int Version { get; set; } = 0;



#pragma warning disable CS8618 
    protected AggregateRootState(AggregateRoot<TID> owner) => Owner = owner;
#pragma warning restore CS8618

#pragma warning disable CS8618
    protected AggregateRootState() { }
#pragma warning restore CS8618
}

