using Domain.SeedWork;

namespace Domain.Entities.Days.Events
{
    public sealed record TaskUpdateMetadata(
            Guid AggregateId,
            int Version,
            DateTimeOffset Timestamp,
            Guid TaskId,
            string Metadata) : IDomainEvent;
}
