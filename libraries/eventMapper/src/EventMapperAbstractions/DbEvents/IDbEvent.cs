using EventMapperAbstractions.Events;

namespace EventMapperAbstractions.DbEvents;

public interface IDbEvent : IEvent<Guid>
{
    public string EventType { get; }
    public string Payload { get; }

    public static IReadOnlySet<string> IgnoredOnPayloadFields
        => new HashSet<string>
        {
            nameof(IEvent<>.AggregateId),
            nameof(IEvent<>.Version),
            nameof(IEvent<>.Timestamp)
        };
}

