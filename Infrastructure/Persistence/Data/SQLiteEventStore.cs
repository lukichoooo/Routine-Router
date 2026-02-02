using Application.Interfaces.Events;
using Domain.SeedWork;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Data.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Data;


public class SQLiteEventStore : IEventStore
{
    private readonly IEventSerializer _serializer;
    private readonly EventsContext _context;

    public SQLiteEventStore(
            IEventSerializer serializer,
            EventsContext context)
    {
        _serializer = serializer;
        _context = context;
    }

    public async Task AppendAsync(
            AggregateRootId aggregateId,
            IReadOnlyCollection<IDomainEvent> events,
            int? expectedVersion,
            CancellationToken ct)
    {
        var currentVersion = await _context.Events
            .FirstOrDefaultAsync(e => e.AggregateId == aggregateId, ct);

        if (currentVersion?.Version != expectedVersion)
        {
            throw new ConcurrencyException(
                $"Expected version {expectedVersion} but found {currentVersion}.");
        }

        IEnumerable<Event> newEvents = events.Select(
                e => Event.From(e, _serializer.Serialize(e)));

        await _context.Events.AddRangeAsync(newEvents);
    }

    public async Task<IReadOnlyCollection<IDomainEvent>> LoadAsync(
            AggregateRootId aggregateId,
            CancellationToken ct,
            int fromVersion = 0,
            int? toVersion = null)
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

