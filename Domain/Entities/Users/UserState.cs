using Domain.Common.ValueObjects;
using Domain.Entities.Users.Events;
using Domain.Entities.Users.ValueObjects;

namespace Domain.Entities.Users;

public class UserState
{
    public UserId Id { get; private set; }
    public Name Name { get; private set; }
    public PasswordHash PasswordHash { get; private set; }


    public void Apply(UserCreated e)
    {
        Id = e.AggregateId;
        Name = e.Name;
        PasswordHash = e.PasswordHash;
    }

    public void Apply(UserUpdated e)
    {
        Name = e.Name;
        PasswordHash = e.PasswordHash;
    }

    public void Apply(UserVerified e) { }


#pragma warning disable CS8618
    public UserState() { }
#pragma warning restore CS8618
}

