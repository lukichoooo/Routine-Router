using Application.Interfaces.Data;
using Application.Interfaces.Events;
using Infrastructure.Persistence.Contexts;

namespace Infrastructure.Persistence;


public sealed class SQLiteUnitOfWork : IUnitOfWork
{
    private readonly EventsContext _context;
    private readonly ITrackedEntities _trackedEntities;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public SQLiteUnitOfWork(
            EventsContext context,
            ITrackedEntities trackedEntities,
            IDomainEventDispatcher domainDispatcher)
    {
        _context = context;
        _trackedEntities = trackedEntities;
        _eventDispatcher = domainDispatcher;
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);

        foreach (var entity in _trackedEntities.GetCollection())
        {
            foreach (var @event in entity.DomainEvents)
                await _eventDispatcher.DispatchAsync(@event, ct);

            entity.ClearDomainEvents();
        }

        _trackedEntities.Clear();
    }
}



