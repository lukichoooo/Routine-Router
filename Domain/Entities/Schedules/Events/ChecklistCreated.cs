using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Schedules.Events;

public sealed record ChecklistCreated(
        ChecklistId AggregateId,
        int Version,
        DateTimeOffset Timestamp,
        UserId UserId
        ) : BaseDomainEvent<ChecklistId>;

