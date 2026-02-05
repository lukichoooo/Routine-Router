using System.Collections.Generic;
using Application.Interfaces.Data;
using Application.Interfaces.Events;
using Domain.Entities.Users;
using Domain.SeedWork;
using Infrastructure.Persistence.Contexts;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;


public sealed class SQLiteUnitOfWork : IUnitOfWork
{
    private readonly EventsContext _eventsContext;
    private readonly EntitiesContext _entitiesContext;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<SQLiteUnitOfWork> _logger;

    public SQLiteUnitOfWork(
            ILogger<SQLiteUnitOfWork> logger,
            EventsContext eventsContext,
            EntitiesContext entitiesContext,
            IDomainEventDispatcher domainDispatcher)
    {
        _logger = logger;
        _eventsContext = eventsContext;
        _entitiesContext = entitiesContext;
        _eventDispatcher = domainDispatcher;
    }

    public async Task CommitAsync(CancellationToken ct)
    {
        await _eventsContext.SaveChangesAsync(ct);
        await _entitiesContext.SaveChangesAsync(ct); // TODO: fix exceptions
        _logger.LogInformation("Commited changes");


        var entries = _entitiesContext.ChangeTracker
            .Entries<IAggregateRootState>()
            .Select(e => e.Entity)
            .ToList();


        _logger.LogInformation($"Entries count: {entries.Count}");
        foreach (var entry in entries)
        {
            _logger.LogInformation($"Entry entity type: {entry.GetType().Name}");
            _logger.LogInformation($"Owner is null: {entry.Owner == null}");
        }
        var entities = entries.ConvertAll(e => e.Owner);
        _logger.LogInformation($"Owners count: {entities.Count}");
        _logger.LogInformation($"Non-null owners: {entities.Count(e => e != null)}");

        foreach (var entity in entities)
        {
            _logger.LogInformation("Dispatching events");
            foreach (var @event in entity.DomainEvents)
            {
                await _eventDispatcher.DispatchAsync(@event, ct); // TODO: async Dispatch
                _logger.LogInformation($"Dispatched event: {@event.GetType().Name}");
            }

            entity.ClearDomainEvents();
        }
    }
}



