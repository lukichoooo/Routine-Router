using Application.Interfaces.Data;
using Domain.SeedWork;

namespace Infrastructure.Persistence;

public sealed class SQLiteUnitOfWork : IUnitOfWork
{
    private readonly RoutineContext _context;
    // private readonly IDomainEventDispatcher _domainDispatcher;

    public SQLiteUnitOfWork(RoutineContext context
    // IDomainEventDispatcher domainDispatcher
    )
    {
        _context = context;
        // _domainDispatcher = domainDispatcher;
    }

    // TODO: WTF
    public async Task CommitAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);

        var entities = _context.ChangeTracker
            .Entries<AggregateRoot<AggregateRootId>>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        foreach (var entity in entities)
        {
            // await _domainDispatcher.DispatchAsync(entity.DomainEvents, ct);
            entity.ClearDomainEvents();
        }
    }
}



