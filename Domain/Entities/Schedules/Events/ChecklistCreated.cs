using Domain.SeedWork;

namespace Domain.Entities.Schedules.Events
{
    public sealed record ChecklistCreated(
            Guid AggregateId,
            int Version,
            DateTimeOffset Timestamp,
            Guid UserId
            ) : IDomainEvent;
}
