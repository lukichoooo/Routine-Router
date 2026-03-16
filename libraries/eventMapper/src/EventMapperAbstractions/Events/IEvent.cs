namespace EventMapperAbstractions.Events;

public interface IEvent;

public interface IEvent<TEntityId> : IEvent
{
    TEntityId AggregateId { get; }
    int Version { get; }
    DateTimeOffset Timestamp { get; }
}
