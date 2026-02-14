using Application.Interfaces.Events;
using Domain.SeedWork;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Data.Exceptions;
using Infrastructure.Persistence.Data.Serializer;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Data;


public class SQLiteEventStore : IEventStore
{
    private readonly IJsonEventMapper _mapper;
    private readonly EventsContext _context;

    public SQLiteEventStore(
            IJsonEventMapper serializer,
            EventsContext context)
    {
        _mapper = serializer;
        _context = context;
    }

    // <summary>
    // Appends events to the event store.
    // expected version shold be null if this is the first event for the aggregate
    // </summary>
    public async Task Append(
            AggregateRootId aggregateId,
            IReadOnlyList<IDomainEvent> events,
            int? expectedVersion,
            CancellationToken ct)
    {
        var onDbEventVersions = _context.Events
            .Where(e => e.AggregateId == aggregateId.Value)
            .Select(e => e.Version);

        int? maxVersion = !await onDbEventVersions.AnyAsync(ct)
            ? null
            : await onDbEventVersions.MaxAsync(ct);

        if (maxVersion != expectedVersion)
        {
            throw new ConcurrencyException(
                $"Expected version: {(dynamic?)expectedVersion ?? "null"} but found: {maxVersion}.");
        }

        IEnumerable<Event> newEvents = events.Select(
                e => Event.From(e, _mapper.ToPayload(e)));

        await _context.Events.AddRangeAsync(newEvents);
    }

    public async Task<List<IDomainEvent>> Load(
            AggregateRootId aggregateId,
            CancellationToken ct,
            int fromVersion = 0,
            int? toVersion = null)
    {
        var dbEvents = await _context.Events
            .AsNoTracking()
            .Where(e => e.AggregateId == aggregateId.Value)
            .OrderBy(e => e.Version)
            .Where(e => e.Version >= fromVersion
                    && (toVersion == null || e.Version <= toVersion))
            .ToListAsync(ct);

        return dbEvents.ConvertAll(_mapper.ToDomainEvent);
    }
}

