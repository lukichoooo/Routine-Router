using MediatR;

namespace Domain.SeedWork;


// <summary>
// Domain Event Interface
// </summary>
public interface IDomainEvent : INotification
{
    AggregateRootId AggregateId { get; }
    int Version { get; }
    DateTimeOffset Timestamp { get; }
}

// <summary>
// BaseDomainEvent AbstractClass
// for domain usage
// </summary>
public abstract record BaseDomainEvent<TAggregateRootId> : IDomainEvent
    where TAggregateRootId : AggregateRootId
{
    public abstract TAggregateRootId AggregateId { get; init; }
    public abstract int Version { get; init; }
    public abstract DateTimeOffset Timestamp { get; init; }

    AggregateRootId IDomainEvent.AggregateId => AggregateId;
}
