using MediatR;

namespace Domain.SeedWork;

public interface IDomainEvent<TAggregateRootId> : INotification
    where TAggregateRootId : AggregateRootId
{
    TAggregateRootId AggregateId { get; }
    int Version { get; }
    DateTimeOffset Timestamp { get; }
}
