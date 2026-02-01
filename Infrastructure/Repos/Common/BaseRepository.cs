using Domain.SeedWork;
using Infrastructure.Persistence;

namespace Infrastructure.Repos.Common;


public abstract class BaseRepository<TA, TID> : IRepository<TA, TID>
where TA : AggregateRoot<TID>
where TID : AggregateRootId
{
    protected readonly ITrackedEntities _trackedEntities;

    protected BaseRepository(ITrackedEntities trackedEntities)
    {
        _trackedEntities = trackedEntities;
    }

    protected abstract Task SaveAsyncProtected(TA aggregate, CancellationToken ct);


    public async Task SaveAsync(TA aggregate, CancellationToken ct)
    {
        await SaveAsyncProtected(aggregate, ct);
        _trackedEntities.Add(aggregate);
    }
    public abstract Task<TA?> GetByIdAsync(TID aggregateId, CancellationToken ct);
}

