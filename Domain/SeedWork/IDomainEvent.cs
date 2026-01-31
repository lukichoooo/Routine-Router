using MediatR;

namespace Domain.SeedWork;

public interface IDomainEvent<TAggregateRootId> : IDomainEvent
    where TAggregateRootId : IAggregateRootId
{
    TAggregateRootId AggregateId { get; }
    int Version { get; }
    DateTimeOffset Timestamp { get; }
}

public interface IDomainEvent : INotification;
