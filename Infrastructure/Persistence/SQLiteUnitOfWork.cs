using Application.Interfaces.Data;
using Application.Interfaces.Events;
using Domain.SeedWork;
using Infrastructure.Persistence.Contexts;

namespace Infrastructure.Persistence;


public sealed class SQLiteUnitOfWork : IUnitOfWork
{
    private readonly EventsContext _eventsContext;
    private readonly EntitiesContext _entitiesContext;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public SQLiteUnitOfWork(
            EventsContext eventsContext,
            EntitiesContext entitiesContext,
            IDomainEventDispatcher domainDispatcher)
    {
        _eventsContext = eventsContext;
        _entitiesContext = entitiesContext;
        _eventDispatcher = domainDispatcher;
    }

    public async Task CommitAsync(CancellationToken ct)
    {
        await _eventsContext.SaveChangesAsync(ct);
        await _entitiesContext.SaveChangesAsync(ct);

        var entities = _entitiesContext.ChangeTracker
            .Entries<AggregateRootState<AggregateRootId>>()
            .Select(e => e.Entity.Owner)
            .ToList();

        foreach (var entity in entities)
        {
            foreach (var @event in entity.DomainEvents)
                await _eventDispatcher.DispatchAsync(@event, ct); // TODO: async Dispatch

            entity.ClearDomainEvents();
        }
    }
}



