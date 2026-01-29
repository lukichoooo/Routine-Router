using Domain.Common.Exceptions;
using Domain.Common.ValueObjects;
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
    }


    public void Update(Name name, PasswordHash passwordHash)
    {
        Name = name ?? throw new DomainArgumentNullException(nameof(name));
        PasswordHash = passwordHash ?? throw new DomainArgumentNullException(nameof(passwordHash));
    }

    public bool Verify(Name name, PasswordHash passwordHash)
        => Name == name && PasswordHash == passwordHash;



#pragma warning disable CS8618 
    private User() { }
#pragma warning restore CS8618
}

