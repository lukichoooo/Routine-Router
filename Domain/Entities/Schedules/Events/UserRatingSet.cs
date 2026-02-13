using Domain.Entities.Schedules.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Schedules.Events;

public sealed record UserRatingSet(
        ChecklistId AggregateId,
        int Version,
        DateTimeOffset Timestamp,
        Rating UserRating) : BaseDomainEvent<ChecklistId>;

