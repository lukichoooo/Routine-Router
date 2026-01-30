using Domain.SeedWork;

namespace Domain.Entities.Days.Events
{
    public sealed record TaskMetadataUpdated(
            Guid AggregateId,
            int Version,
            DateTimeOffset Timestamp,
            Guid TaskId,
            string Metadata) : IDomainEvent;
}
