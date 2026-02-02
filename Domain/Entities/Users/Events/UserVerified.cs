using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Users.Events
{
    public sealed record UserVerified(
            UserId AggregateId,
            int Version,
            DateTimeOffset Timestamp
            ) : BaseDomainEvent<UserId>;
}
