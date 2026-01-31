using MediatR;

namespace Domain.SeedWork;

public interface IDomainEvent<TAggregateId> : INotification
{
    TAggregateId AggregateId { get; }
    int Version { get; }
    DateTimeOffset Timestamp { get; }
}

