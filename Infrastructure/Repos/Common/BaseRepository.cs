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
where TId : EntityId
where TState : AggregateRootState<TId>, IAggregateRootStateFactory<TState, TId>

{
    // Save
    public async Task Save(TEntity aggregate, CancellationToken ct)
    {
        await eventStore.Append(
                aggregate.Id,
                aggregate.DomainEvents,
                expectedVersion: aggregate.StoredVersion,
                ct);

        await stateStore.Add(aggregate.State, ct);
    }

    // Query
    public async Task<TEntity?> GetById(TId aggregateId, CancellationToken ct)
    {
        var state = await stateStore.Get(aggregateId, ct);
        if (state is not null)
            return TEntity.Create(state);

        var events = await eventStore.Load(aggregateId, ct);
        if (events.Count == 0)
            return null;

        var entity = TEntity.Create(events);
        await stateStore.Add(entity.State, ct);

        return entity;
    }

}

