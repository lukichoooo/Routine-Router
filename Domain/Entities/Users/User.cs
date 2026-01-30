using Domain.Common;
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
    public User(IEnumerable<IDomainEvent>? events = null)
    {
        State = new UserState();
        foreach (var e in events ?? [])
        {
            ((dynamic)State).Apply((dynamic)e);
            Version = e.Version;
        }
    }

    private void AppendEvent(IDomainEvent e)
    {
        AddDomainEvent(e);
        ((dynamic)State).Apply((dynamic)e);
        Version = e.Version;
    }

    // ---------- COMMANDS ----------

    public void Create(Name name, PasswordHash passwordHash)
        => AppendEvent(new UserCreated(
                    AggregateId: State.Id,
                    Version: GetNextVersion,
                    Timestamp: Clock.Now,
                    name,
                    passwordHash
        ));

    public void Update(Name name, PasswordHash passwordHash)
    {
        if (name == State.Name && passwordHash == State.PasswordHash)
            throw new DomainArgumentException("User Update has same fields");

        AppendEvent(new UserUpdated(
                    AggregateId: State.Id,
                    Version: GetNextVersion,
                    Timestamp: Clock.Now,
                    name,
                    passwordHash
                    ));
    }

    public void Verify(Name name, PasswordHash passwordHash)
    {
        if (State.Name != name || State.PasswordHash != passwordHash)
            throw new WrongUserCredentialsException();

        AppendEvent(new UserVerified(
                    AggregateId: State.Id,
                    Version: GetNextVersion,
                    Timestamp: Clock.Now
                    ));
    }


}

