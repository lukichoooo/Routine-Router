using Domain.SeedWork;

namespace Domain.Entities.Users.ValueObjects;


public class UserId : EntityId
{
    public UserId(Guid value) : base(value) { }

    private UserId() { }
}

