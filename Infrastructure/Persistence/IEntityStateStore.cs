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

    Task Save(TState aggregateState, CancellationToken ct);
}


public class SQLiteStateStore<TState, TId>(EntitiesContext context) : IEntityStateStore<TState, TId>
where TId : EntityId
where TState : AggregateRootState<TId>
{
    public async Task<TState?> Get(TId aggregateId, CancellationToken ct)
        => await context
        .Set<TState>()
        .FindAsync([aggregateId], ct);

    public async Task Save(TState aggregateState, CancellationToken ct)
    {
        var existing = await context.Set<TState>().FindAsync([aggregateState.Id], ct);
        if (existing is not null)
            context.Entry(existing).CurrentValues.SetValues(aggregateState);
        else
            await context.AddAsync(aggregateState, ct);
    }
}

