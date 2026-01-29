using Domain.Common.Exceptions;
using Domain.Common.ValueObjects;
using Domain.Entities.Days.Events;
using Domain.Entities.Days.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Days;

public class Checklist : Entity, IAggregateRoot
{
    private readonly List<Task> _tasks = [];
    public IReadOnlyCollection<Task> Tasks => _tasks;

    public bool Completed { get; private set; }

    public Statistics Statistics { get; private set; }

    public Guid UserId { get; private set; }


    public Checklist(Guid userId)
    {
        UserId = userId;
        Statistics = new(DateOnly.FromDateTime(DateTime.Now));

        AddDomainEvent(new ChecklistCreated(Id, userId));
    }



    public void AddTask(
        Name name,
        Schedule planned,
        string? metadata = null)
    {
        var task = new Task(name, planned, Id, metadata);
        _tasks.Add(task);

        AddDomainEvent(new TaskAddedToChecklist(
                    ChecklistId: Id,
                    TaskId: task.Id));
    }

    public void RemoveTask(Guid TaskId)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == TaskId)
            ?? throw new DomainArgumentException($"Task with id {TaskId} not found");
        _tasks.Remove(task);

        AddDomainEvent(new TaskRemovedFromChecklist(
                    ChecklistId: Id,
                    TaskId: task.Id));
    }

    public void CompleteTask(Guid TaskId)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == TaskId)
            ?? throw new DomainArgumentException($"Task with id {TaskId} not found");
        task.Complete();
    }

    public void StartTask(Guid TaskId)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == TaskId)
            ?? throw new DomainArgumentException($"Task with id {TaskId} not found");
        task.Start();
    }

    public void UpdateTaskMetadata(Guid TaskId, string metadata)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == TaskId)
            ?? throw new DomainArgumentException($"Task with id {TaskId} not found");
        task.UpdateMetadata(metadata);
    }


    public void SetUserRating(Rating userRating)
        => Statistics = Statistics.WithUserRating(userRating);

    public void SetLLMRating(Rating llmRating)
        => Statistics = Statistics.WithLLMRating(llmRating);



#pragma warning disable CS8618 
    private Checklist() { }
#pragma warning restore CS8618
}


