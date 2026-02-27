using Domain.Common.ValueObjects;
using Domain.Entities.Users.Events;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Users;

public sealed class UserState : AggregateRootState<UserId>, IAggregateRootStateFactory<UserState, UserId>
{
    public Name Name { get; private set; }
    public PasswordHash PasswordHash { get; private set; }


    public static UserState CreateState(AggregateRoot<UserId> owner)
        => new(owner);


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
    private UserState(AggregateRoot<UserId> owner) : base(owner) { }
#pragma warning restore CS8618

#pragma warning disable CS8618
    private UserState() { }
#pragma warning restore CS8618
}

