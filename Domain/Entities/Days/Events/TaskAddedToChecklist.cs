using Domain.Common.ValueObjects;
using Domain.Entities.Days.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Days.Events
{
    public sealed record TaskAddedToChecklist(
            Guid AggregateId,
            int Version,
            DateTimeOffset Timestamp,
            Guid TaskId,
            Name Name,
            TaskType TaskType,
            Schedule Planned,
            string? Metadata
         ) : IDomainEvent;
}
