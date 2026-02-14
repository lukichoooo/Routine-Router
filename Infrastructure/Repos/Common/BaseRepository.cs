using Application.Interfaces.Events;
using Domain.SeedWork;
using Infrastructure.Persistence;

namespace Infrastructure.Repos.Common;


// <summary>
// Base Repository for Infrastructure Repos
// automatically manages state storage
// </summary>
public abstract class BaseRepository<TEntity, TId, TState>
(IEntityStateStore<TState, TId> stateStore, IEventStore eventStore) : IRepository<TEntity, TId>

where TEntity : AggregateRoot<TId, TState>, IEntityFactory<TEntity, TId, TState>
where TId : AggregateRootId
where TState : AggregateRootState<TId>, IAggregateRootStateFactory<TState, TId>

{
    // Save
    public async Task AddAsync(TEntity aggregate, CancellationToken ct)
    {
        await eventStore.AppendAsync(
                aggregate.Id,
                aggregate.DomainEvents,
                expectedVersion: aggregate.StoredVersion,
                ct);

        await stateStore.AddAsync(aggregate.State, ct);
    }

    // Query
    public async Task<TEntity?> GetByIdAsync(TId aggregateId, CancellationToken ct)
    {
        var state = await stateStore.GetAsync(aggregateId, ct);
        if (state is not null)
            return TEntity.Create(ref state);

        var events = await eventStore.LoadAsync(aggregateId, ct);
        if (events.Count == 0)
            return null;

        var entity = TEntity.Create(events);
        await stateStore.AddAsync(entity.State, ct);

        return entity;
    }

}

