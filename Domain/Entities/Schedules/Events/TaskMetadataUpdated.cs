using Domain.SeedWork;

namespace Domain.Entities.Schedules.Events
{
    public sealed record TaskMetadataUpdated(
            Guid AggregateId,
            int Version,
            DateTimeOffset Timestamp,
            Guid TaskId,
            string Metadata) : IDomainEvent;
}
