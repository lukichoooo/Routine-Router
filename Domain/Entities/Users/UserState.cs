using Domain.Common.ValueObjects;
using Domain.Entities.Users.Events;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Users;

public class UserState : AggregateRootState<UserId>, IAggregateRootStateFactory<UserState, UserId>
{
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


    public static UserState CreateState(AggregateRoot<UserId> owner)
    {
        return new(owner);
    }


#pragma warning disable CS8618 
    private UserState(AggregateRoot<UserId> owner) : base(owner) { }
#pragma warning restore CS8618

#pragma warning disable CS8618
    private UserState() { }
#pragma warning restore CS8618
}

