using Domain.SeedWork;

namespace Domain.Entities.Schedules.Events
{
    public sealed record TaskStarted(
            Guid AggregateId,
            int Version,
            DateTimeOffset Timestamp,
            Guid TaskId
            ) : IDomainEvent;
}
