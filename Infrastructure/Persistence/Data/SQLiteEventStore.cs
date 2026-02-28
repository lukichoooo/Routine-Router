using Application.Interfaces.Events;
using Domain.SeedWork;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Data.Exceptions;
using Infrastructure.Persistence.Data.Serializer;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Data;


public class SQLiteEventStore(
        IJsonEventMapper mapper,
        EventContext context) : IEventStore
{
    // <summary>
    // Appends events to the event store.
    // expected version shold be null if this is the first event for the aggregate
    // </summary>
    public async Task Append(
            EntityId aggregateId,
            IReadOnlyList<IDomainEvent> events,
            int? expectedVersion,
            CancellationToken ct)
    {
        var onDbEventVersions = context.Events
            .Where(e => e.AggregateId == aggregateId.Value)
            .Select(e => e.Version);

        int? maxVersion = !await onDbEventVersions.AnyAsync(ct)
            ? null
            : await onDbEventVersions.MaxAsync(ct);

        if (maxVersion != expectedVersion)
        {
            throw new ConcurrencyException(@$"
                Expected version: {expectedVersion} 
                but found: {maxVersion}.");
        }

        IEnumerable<Event> newEvents = events
            .Select(e => Event.From(e, mapper.ToPayload(e)));

        await context.Events.AddRangeAsync(newEvents, ct);
    }

    public async Task<List<IDomainEvent>> Load(
            EntityId aggregateId,
            CancellationToken ct,
            int fromVersion = 0,
            int? toVersion = null)
    {
        var dbEvents = await context.Events
            .AsNoTracking()
            .Where(e => e.AggregateId == aggregateId.Value)
            .OrderBy(e => e.Version)
            .Where(e => e.Version >= fromVersion
                    && (toVersion == null || e.Version <= toVersion))
            .ToListAsync(ct);

        return dbEvents.ConvertAll(mapper.ToDomainEvent);
    }
}

