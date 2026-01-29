using Domain.Common.ValueObjects;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Users.Events
{
    public sealed record UserUpdated(
            Guid UserId,
            Name Name,
            PasswordHash PasswordHash) : IDomainEvent;
}
