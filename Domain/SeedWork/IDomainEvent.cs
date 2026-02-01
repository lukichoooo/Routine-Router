using MediatR;

namespace Domain.SeedWork;


// <summary>
// Domain Event
// </summary>
public interface IDomainEvent<out TAggregateRootId> : INotification
    where TAggregateRootId : AggregateRootId
{
    TAggregateRootId AggregateId { get; }
    int Version { get; }
    DateTimeOffset Timestamp { get; }
}
