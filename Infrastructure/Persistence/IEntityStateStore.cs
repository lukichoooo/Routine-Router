using Domain.SeedWork;
using Infrastructure.Persistence.Contexts;

namespace Infrastructure.Persistence;


// <summary>
// only saves most recent version of each entity
// uses batching
// </summary>
public interface IEntityStateStore<TState, TId>
        where TState : AggregateRootState<TId>
        where TId : EntityId
{
    Task<TState?> Get(TId aggregateId, CancellationToken ct);

    Task Add(TState aggregateState, CancellationToken ct);
}


public class SQLiteStateStore<TState, TId>(EntitiesContext context) : IEntityStateStore<TState, TId>
where TId : EntityId
where TState : AggregateRootState<TId>
{
    public async Task<TState?> Get(TId aggregateId, CancellationToken ct)
        => await context
        .Set<TState>()
        .FindAsync([aggregateId], ct);

    public async Task Add(TState aggregateState, CancellationToken ct)
        => await context.AddAsync(aggregateState, ct);
}

