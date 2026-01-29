using Domain.Entities.Days.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Days.Events
{
    public sealed record UserRatingSet(
            Guid ChecklistId,
            Rating UserRating) : IDomainEvent;
}
