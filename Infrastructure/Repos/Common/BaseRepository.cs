using Domain.SeedWork;
using Infrastructure.Persistence;

namespace Infrastructure.Repos.Common;


// <summary>
// Base Repository for Infrastructure Repos
// automatically manages state storage
// </summary>
public abstract class BaseRepository<TA, TID, TS> : IRepository<TA, TID>
where TA : AggregateRoot<TID, TS>
where TID : AggregateRootId
where TS : AggregateRootState<TID>, IAggregateRootStateFactory<TS, TID>
{
    protected readonly IEntityStateStore<TS, TID> _stateStore;

    protected BaseRepository(IEntityStateStore<TS, TID> entityStore)
    {
        _stateStore = entityStore;
    }

    // Save
    public async Task AddAsync(TA aggregate, CancellationToken ct)
    {
        await SaveEventsAsync(aggregate, ct);
        await _stateStore.AddAsync(aggregate.State, ct);
    }
    protected abstract Task SaveEventsAsync(TA aggregate, CancellationToken ct);

    // Query
    public abstract Task<TA?> GetByIdAsync(TID aggregateId, CancellationToken ct);

}

