using Domain.Common.ValueObjects;
using Domain.Entities.Schedules.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Schedules.Events;

public sealed record TaskAddedToChecklist(
        ChecklistId AggregateId,
        int Version,
        DateTimeOffset Timestamp,
        TaskId TaskId,
        Name Name,
        TaskType TaskType,
        Schedule Planned,
        string? Metadata
     ) : BaseDomainEvent<ChecklistId>;

