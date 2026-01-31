using Application.Interfaces.Events;
using Domain.SeedWork;

namespace Infrastructure.Persistence.Data;


public class SQLiteEventStore<TAggregateRootId> : IEventStore<TAggregateRootId>
    where TAggregateRootId : IAggregateRootId
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
            TAggregateRootId aggregateId,
            IReadOnlyCollection<IDomainEvent<TAggregateRootId>> events,
            int expectedVersion,
            CancellationToken ct)
    {
        IEnumerable<Event> dbEvents = events.Select(e =>
                new Event()
                {
                    AggregateId = aggregateId.Value,
                    AggregateIdType = aggregateId.GetType().Name,
                    Version = e.Version,
                    EventType = e.GetType().Name,
                    EventData = _serializer.Serialize(e),
                    TimeStamp = e.Timestamp
                });
        await _context.Events.AddRangeAsync(dbEvents);
    }

    public Task<IReadOnlyCollection<IDomainEvent<TAggregateRootId>>> LoadAsync(
            TAggregateRootId aggregateId,
            CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<IDomainEvent<TAggregateRootId>>> LoadAsync(
            TAggregateRootId aggregateId,
            int fromVersion,
            CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}

