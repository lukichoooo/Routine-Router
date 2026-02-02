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

    public async Task AppendAsync(
            AggregateRootId aggregateId,
            IReadOnlyCollection<IDomainEvent> events,
            int? expectedVersion,
            CancellationToken ct)
    {
        var currentVersion = await _context.Events
            .FirstOrDefaultAsync(e => e.AggregateId == aggregateId.Value, ct);

        if (currentVersion?.Version != expectedVersion)
        {
            throw new ConcurrencyException(
                $"Expected version {expectedVersion} but found {currentVersion}.");
        }

        IEnumerable<Event> newEvents = events.Select(
                e => Event.From(e, _mapper.ToPayload(e)));

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
            .Where(e => e.AggregateId == aggregateId.Value)
            .OrderBy(e => e.Version)
            .Where(e => e.Version >= fromVersion
                    && (toVersion == null || e.Version <= toVersion));

        return dbEvents
            .Select(_mapper.ToDomainEvent)
            .ToList()
            .AsReadOnly();
    }
}

