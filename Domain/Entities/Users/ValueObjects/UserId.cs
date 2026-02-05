using Domain.SeedWork;

namespace Domain.Entities.Users.ValueObjects;


public class UserId : AggregateRootId
{
    public UserId(Guid value) : base(value) { }

    public UserId() { }
}

