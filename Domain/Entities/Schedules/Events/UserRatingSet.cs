using Domain.Entities.Schedules.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Schedules.Events
{
    public sealed record UserRatingSet(
            Guid AggregateId,
            int Version,
            DateTimeOffset Timestamp,
            Rating UserRating) : IDomainEvent;
}
