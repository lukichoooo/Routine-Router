using Domain.Entities.Schedules.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Schedules.Events
{
    public sealed record LLMRatingSet(
            Guid AggregateId,
            int Version,
            DateTimeOffset Timestamp,
            Rating LLMRating
            ) : IDomainEvent;
}
