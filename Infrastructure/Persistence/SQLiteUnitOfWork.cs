using System.Data;
using System.Transactions;
using Application.Interfaces.Data;
using Application.Interfaces.Events;
using Domain.SeedWork;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;


public sealed class SQLiteUnitOfWork(
        ILogger<SQLiteUnitOfWork> logger,
        EventsContext eventsContext,
        EntitiesContext entitiesContext,
        IDomainEventDispatcher domainEventDispatcher) : IUnitOfWork
{
    public async Task Commit(CancellationToken ct)
    {
        // TODO: Test Atomic Transactoin
        using (var scope = new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions
                    {
                        IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                        Timeout = TimeSpan.FromSeconds(30),
                    },
                    TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                await entitiesContext.Database.OpenConnectionAsync(ct);
                await eventsContext.Database.OpenConnectionAsync(ct);
                scope.Complete();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Commit failed");
                throw;
            }
        }

        logger.LogInformation("Commited changes");


        var states = entitiesContext.ChangeTracker
            .Entries<IAggregateRootState>()
            .Select(e => e.Entity)
            .ToList();


        logger.LogInformation($"Entries count: {states.Count}");
        foreach (var entry in states)
        {
            logger.LogInformation($"Entry entity type: {entry.GetType().Name}");
            logger.LogInformation($"Owner is null: {entry.Owner == null}");
        }

        var entities = states.ConvertAll(e => e.Owner);
        logger.LogInformation($"Owners count: {entities.Count}");
        logger.LogInformation($"Non-null owners: {entities.Count(e => e != null)}");

        foreach (var entity in entities)
        {
            if (entity is null)
            {
                logger.LogInformation($"Entity:{entity} is null");
                continue;
            }

            logger.LogInformation($"Dispatching events for Entity:{entity}");
            foreach (var @event in entity.DomainEvents)
            {
                await domainEventDispatcher.Dispatch(@event, ct); // TODO: async Dispatch
                logger.LogInformation($"Dispatched event: {@event.GetType().Name}");
            }

            entity.ClearDomainEvents();
        }
    }
}



