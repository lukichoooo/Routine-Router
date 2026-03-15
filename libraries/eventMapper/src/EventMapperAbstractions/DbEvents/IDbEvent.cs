using EventMapperAbstractions.Events;

namespace EventMapperAbstractions.DbEvents;

public interface IDbEvent
{
    public static IReadOnlySet<string> IgnoredOnPayloadFields
        => new HashSet<string>
        {
            nameof(IEvent<>.AggregateId),
            nameof(IEvent<>.Version),
            nameof(IEvent<>.Timestamp)
        };
}

