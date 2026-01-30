using MediatR;

namespace Domain.SeedWork;

public interface IDomainEvent : INotification
{
    Guid AggregateId { get; }
    int Version { get; }
    DateTimeOffset Timestamp { get; }
}

