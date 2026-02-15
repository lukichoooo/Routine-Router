using MediatR;

namespace Domain.SeedWork;


// <summary>
// Domain Event Interface
// </summary>
public interface IDomainEvent : INotification
{
    EntityId AggregateId { get; }
    int Version { get; }
    DateTimeOffset Timestamp { get; }
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

    EntityId IDomainEvent.AggregateId => AggregateId;
}
