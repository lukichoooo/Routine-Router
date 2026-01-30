using Domain.SeedWork;

namespace Domain.Entities.Days.Events
{
    public sealed record TaskRemovedFromChecklist(
            Guid AggregateId,
            int Version,
            DateTimeOffset Timestamp,
            Guid TaskId) : IDomainEvent;
}
