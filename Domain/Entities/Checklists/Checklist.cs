using Domain.SeedWork;

namespace Domain.Entities.Checklists;

public class Checklist : Entity, IAggregateRoot
{
    private readonly List<Task> _tasks = [];
    public IReadOnlyCollection<Task> Tasks => _tasks.AsReadOnly();

    public bool Completed { get; private set; }



    private Checklist() { }
}


