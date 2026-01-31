using Domain.Entities.Schedules.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Schedules.Events
{
    public sealed record TaskMetadataUpdated(
            ChecklistId AggregateId,
            int Version,
            DateTimeOffset Timestamp,
            TaskId TaskId,
            string Metadata) : IDomainEvent<ChecklistId>;
}
