using EventMapperAbstractions.DbEvents;
using EventMapperAbstractions.Events;
using EventMapperAbstractions.SeedWork;

namespace Unit.EventMapperGeneration;

public sealed class TestEntityId(Guid value) : IAggregateId
{
    public Guid Value { get; set; } = value;
    public Guid ToGuid() => Value;
}


public class Event : IDbEvent
{
    public int Id { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public required Guid AggregateId { get; set; }
    public required int Version { get; set; }
    public required DateTimeOffset Timestamp { get; set; }
    public required string AggregateIdType { get; set; }
    public required string EventType { get; set; }
    public required string Payload { get; set; }
    public static Event From(IEvent<TestEntityId> e, string payload)
        => new()
        {
            AggregateId = e.AggregateId.ToGuid(),
            AggregateIdType = e.AggregateId.GetType().Name,
            EventType = e.GetType().Name,
            Version = e.Version,
            Payload = payload,
            Timestamp = e.Timestamp
        };
}

