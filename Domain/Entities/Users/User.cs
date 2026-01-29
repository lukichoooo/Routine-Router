using Domain.Common.Exceptions;
using Domain.Common.ValueObjects;
using Domain.Entities.Users.Events;
using Domain.Entities.Users.Exceptions;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Users;

public sealed class User : AggregateRoot
{
    private readonly UserState State;

    // ---------- FACTORY ----------
    public User(IEnumerable<IDomainEvent> events)
    {
        State = new UserState();
        foreach (var e in events)
        {
            AppendEvent(e);
        }
    }

    private void AppendEvent(IDomainEvent e)
    {
        AddDomainEvent(e);
        ((dynamic)State).Apply((dynamic)e);
    }

    // ---------- COMMANDS ----------

    public void Create(Name name, PasswordHash passwordHash)
        => AppendEvent(new UserCreated(
                    Guid.NewGuid(),
                    name,
                    passwordHash
        ));

    public void Update(Name name, PasswordHash passwordHash)
    {
        if (name == State.Name && passwordHash == State.PasswordHash)
            throw new DomainArgumentException("User Update has same fields");

        AppendEvent(new UserUpdated(State.Id, name, passwordHash));
    }

    public void Verify(Name name, PasswordHash passwordHash)
    {
        if (State.Name != name || State.PasswordHash != passwordHash)
            throw new WrongUserCredentialsException();

        AppendEvent(new UserVerified(State.Id));
    }


}

