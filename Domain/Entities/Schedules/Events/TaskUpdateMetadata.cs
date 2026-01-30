using Domain.SeedWork;

namespace Domain.Entities.Schedules.Events
{
    public sealed record TaskUpdateMetadata(
            Guid AggregateId,
            int Version,
            DateTimeOffset Timestamp,
            Guid TaskId,
            string Metadata) : IDomainEvent;
}
