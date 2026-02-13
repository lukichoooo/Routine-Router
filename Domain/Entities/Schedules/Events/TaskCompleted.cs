using Domain.Entities.Schedules.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Schedules.Events;

public sealed record TaskCompleted(
        ChecklistId AggregateId,
        int Version,
        DateTimeOffset Timestamp,
        TaskId TaskId
        ) : BaseDomainEvent<ChecklistId>;

