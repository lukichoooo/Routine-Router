using Domain.SeedWork;
using Infrastructure.Persistence.Contexts;

namespace Infrastructure.Persistence;


// <summary>
// does NOT call SaveChanges()
// only saves most recent version of each entity
// uses batching
// </summary>
public interface IEntityStateStore<TState, TId>
        where TState : AggregateRootState<TId>
        where TId : AggregateRootId
{
    Task<TState?> GetAsync(TId aggregateId, CancellationToken ct);

    Task Add(TState aggregateState, CancellationToken ct);
}


public class SQLiteStateStore<TState, TId>(EntitiesContext context) : IEntityStateStore<TState, TId>
where TId : AggregateRootId
where TState : AggregateRootState<TId>
{
    private readonly EntitiesContext _context = context;

    public async Task<TState?> GetAsync(TId aggregateId, CancellationToken ct)
        => await _context
        .Set<TState>()
        .FindAsync([aggregateId], ct);

    public async Task Add(TState aggregateState, CancellationToken ct)
        => await _context.AddAsync(aggregateState, ct);
}

