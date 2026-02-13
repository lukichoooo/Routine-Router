using Domain.Common.ValueObjects;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Users.Events;

public sealed record UserUpdated(
        UserId AggregateId,
        int Version,
        DateTimeOffset Timestamp,
        Name Name,
        PasswordHash PasswordHash) : BaseDomainEvent<UserId>;

