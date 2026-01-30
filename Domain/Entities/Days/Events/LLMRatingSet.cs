using Domain.Entities.Days.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Days.Events
{
    public sealed record LLMRatingSet(
            Guid AggregateId,
            int Version,
            DateTimeOffset Timestamp,
            Rating LLMRating
            ) : IDomainEvent;
}
