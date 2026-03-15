using EventMapperAbstractions.Events;

namespace EventMapperAbstractions.DbEvents;

public interface IDbEvent : IEvent<Guid>
{
    public string EventType { get; }
    public string Payload { get; }
}

