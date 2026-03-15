using Mediator;
using EventMapperAbstractions.Events;

namespace Domain.SeedWork;


// <summary>
// Domain Event Interface
// </summary>
public interface IDomainEvent : INotification, IEvent<EntityId>
{
    new EntityId AggregateId { get; }
    new int Version { get; }
    new DateTimeOffset Timestamp { get; }
}

// <summary>
// Abstract Class for domain usage
// </summary>
public abstract record BaseDomainEvent<TAggregateRootId> : IDomainEvent
    where TAggregateRootId : EntityId
{
    public abstract TAggregateRootId AggregateId { get; init; }
    public abstract int Version { get; init; }
    public abstract DateTimeOffset Timestamp { get; init; }


    // Base interfaces inheritance
    EntityId IDomainEvent.AggregateId => AggregateId;
    EntityId IEvent<EntityId>.AggregateId => AggregateId;
}
