using Domain.SeedWork;

namespace Domain.Entities.Days.Events
{
    public sealed record TaskCompleted(
            Guid AggregateId,
            int Version,
            DateTimeOffset Timestamp,
            Guid TaskId
            ) : IDomainEvent;
}
