using Domain.Common.Exceptions;
using Domain.Common.ValueObjects;
using Domain.Entities.Days.Events;
using Domain.SeedWork;

namespace Domain.Entities.Days;

public class DailyChecklist : Entity, IAggregateRoot
{
    private readonly List<Task> _tasks = [];
    public IReadOnlyCollection<Task> Tasks => _tasks;

    public bool Completed { get; private set; }

    public Statistics Statistics { get; private set; }

    public Guid UserId { get; private set; }


    public DailyChecklist(
            Guid userId,
            Statistics statistics)
    {
        UserId = userId;
        Statistics = statistics ?? throw new DomainArgumentNullException(nameof(statistics));
    }



    public void AddTask(
        Name name,
        Schedule planned,
        string? metadata = null)
    {
        var task = new Task(name, planned, metadata);
        _tasks.Add(task);

        AddDomainEvent(new TaskAddedToChecklist(Id, task.Id));
    }

    public void RemoveTask(Guid TaskId)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == TaskId)
            ?? throw new DomainArgumentException($"Task with id {TaskId} not found");
        _tasks.Remove(task);
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
        => Statistics.SetUserRating(userRating);

    public void SetLLMRating(Rating llmRating)
        => Statistics.SetLLMRating(llmRating);



#pragma warning disable CS8618 
    private DailyChecklist() { }
#pragma warning restore CS8618
}


