using Domain.Common.Exceptions;
using Domain.Common.ValueObjects;
using Domain.Entities.Users.Events;
using Domain.Entities.Users.Exceptions;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Users;

public sealed class User : Entity, IAggregateRoot
{
    public Name Name { get; private set; }
    public PasswordHash PasswordHash { get; private set; }

    public User(Name name, PasswordHash passwordHash)
    {
        Name = name ?? throw new DomainArgumentNullException(nameof(name));
        PasswordHash = passwordHash ?? throw new DomainArgumentNullException(nameof(passwordHash));

        AddDomainEvent(new UserCreated(Id, Name, PasswordHash));
    }


    public void Update(Name name, PasswordHash passwordHash)
    {
        Name = name ?? throw new DomainArgumentNullException(nameof(name));
        PasswordHash = passwordHash ?? throw new DomainArgumentNullException(nameof(passwordHash));

        AddDomainEvent(new UserUpdated(Id, Name, PasswordHash));
    }

    public void Verify(Name name, PasswordHash passwordHash)
    {
        if (Name != name || PasswordHash != passwordHash)
            throw new WrongUserCredentialsException();

        AddDomainEvent(new UserVerified(Id));
    }



#pragma warning disable CS8618 
    private User() { }
#pragma warning restore CS8618
}

