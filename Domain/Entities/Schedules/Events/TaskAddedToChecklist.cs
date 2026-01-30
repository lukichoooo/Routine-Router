using Domain.Common.ValueObjects;
using Domain.Entities.Schedules.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Schedules.Events
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
