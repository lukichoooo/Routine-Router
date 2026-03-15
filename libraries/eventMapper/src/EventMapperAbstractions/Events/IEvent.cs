namespace EventMapperAbstractions.Events;

public interface IEvent<TEntityId>
{
    TEntityId AggregateId { get; }
    int Version { get; }
    DateTimeOffset Timestamp { get; }
}
