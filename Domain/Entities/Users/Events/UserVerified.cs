using Domain.SeedWork;

namespace Domain.Entities.Users.Events
{
    public sealed record UserVerified(
            Guid AggregateId,
            int Version,
            DateTimeOffset Timestamp
            ) : IDomainEvent;
}
