using Domain.Entities.Times;
using Domain.SeedWork;

namespace Domain.Entities.Users;

public class User : Entity, IAggregateRoot
{
    public string Name { get; private set; }

    public List<Day> Days { get; private set; } = [];
}

