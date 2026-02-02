namespace Domain.SeedWork;


public interface IState<TID> where TID : AggregateRootId
{
    public TID Id { get; }
}

