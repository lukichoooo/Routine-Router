using Application.Interfaces.Events;
using Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Data;


public class SQLiteEventStore : IEventStore
{
    private readonly IEventSerializer _serializer;
    private readonly RoutineContext _context;

    public SQLiteEventStore(
            IEventSerializer serializer,
            RoutineContext context)
    {
        _serializer = serializer;
        _context = context;
    }

    public async Task AppendAsync(
            IReadOnlyCollection<IDomainEvent<AggregateRootId>> events,
            int expectedVersion,
            CancellationToken ct)
    {
        IEnumerable<Event> dbEvents = events.Select(e => Event.From(e, _serializer.Serialize(e)));
        await _context.Events.AddRangeAsync(dbEvents);
    }

    public async Task<IReadOnlyCollection<IDomainEvent<AggregateRootId>>> LoadAsync(
            AggregateRootId aggregateId,
            CancellationToken ct)
    {
        var dbEvents = _context.Events
            .AsNoTracking()
            .Where(e => e.AggregateId == aggregateId)
            .OrderBy(e => e.Version);

        return await dbEvents
            .Select(e => _serializer.Deserialize(e.EventData, e.EventType))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<IDomainEvent<AggregateRootId>>> LoadAsync(
            AggregateRootId aggregateId,
             int fromVersion = 0,
            int? toVersion = null,
            CancellationToken ct = default)
    {
        var dbEvents = _context.Events
            .AsNoTracking()
            .Where(e => e.AggregateId == aggregateId)
            .OrderBy(e => e.Version)
            .Where(e => e.Version >= fromVersion
                    && (toVersion == null || e.Version <= toVersion));

        return await dbEvents
            .Select(e => _serializer.Deserialize(e.EventData, e.EventType))
            .ToListAsync(ct);
    }
}

