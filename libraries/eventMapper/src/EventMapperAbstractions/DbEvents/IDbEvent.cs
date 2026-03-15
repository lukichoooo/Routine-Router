namespace EventMapperAbstractions.DbEvents;

public interface IDbEvent
{
    Guid AggregateId { get; }
    int Version { get; }
    DateTimeOffset Timestamp { get; }

    public string EventType { get; }
    public string Payload { get; }
}

