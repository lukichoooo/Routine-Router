namespace Domain.SeedWork;


public abstract class State<TID> where TID : AggregateRootId
{
    public TID Id { get; protected set; }
    public int Version { get; set; } = 0;

#pragma warning disable CS8618
    protected State() { }
#pragma warning restore CS8618
}

