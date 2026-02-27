using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Common.ValueObjects;
using Domain.Entities.Users.Events;
using Domain.Entities.Users.Exceptions;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Users;

public sealed class User :
    AggregateRoot<UserId, UserState>,
    IEntityFactory<User, UserId, UserState>
{
    public User(IEnumerable<IDomainEvent>? history = null) : base(history) { }
    public User(UserState state) : base(state) { }
    public static User Create(UserState storedState) => new(storedState);
    public static User Create(IEnumerable<IDomainEvent>? history) => new(history);


    public void Create(UserId id, Name name, PasswordHash passwordHash)
        => AppendEvent(new UserCreated(
                    AggregateId: id,
                    Version: NextVersion,
                    Timestamp: Clock.CurrentTime,
                    name,
                    passwordHash
        ));

    public void Update(Name name, PasswordHash passwordHash)
    {
        if (name == State.Name && passwordHash == State.PasswordHash)
            throw new DomainArgumentException("User Update has same fields");

        AppendEvent(new UserUpdated(
                    AggregateId: Id,
                    Version: NextVersion,
                    Timestamp: Clock.CurrentTime,
                    name,
                    passwordHash
                    ));
    }

    public void Verify(Name name, PasswordHash passwordHash)
    {
        if (State.Name != name || State.PasswordHash != passwordHash)
            throw new WrongUserCredentialsException();

        AppendEvent(new UserVerified(
                    AggregateId: Id,
                    Version: NextVersion,
                    Timestamp: Clock.CurrentTime
                    ));
    }

    private User() { }
}

