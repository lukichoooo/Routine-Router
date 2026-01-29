using Domain.Common.Exceptions;
using Domain.Common.ValueObjects;
using Domain.Entities.Days.Events;
using Domain.Entities.Days.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Days;

public sealed class Checklist : AggregateRoot
{
    private readonly List<Task> _tasks = [];

    public IReadOnlyCollection<Task> Tasks => _tasks;
    public Guid UserId { get; private set; }
    public Statistics Statistics { get; private set; } = null!;

    // ---------- FACTORY ----------
    public static Checklist Create(Guid userId)
    {
        var checklist = new Checklist();
        checklist.AddDomainEvent(new ChecklistCreated(
            ChecklistId: checklist.Id,
            UserId: userId,
            CreatedAt: DateTime.UtcNow
        ));
        return checklist;
    }

    // ---------- COMMANDS (WRITE LOGIC) ----------

    public void AddTask(Name name, TaskType taskType, Schedule planned, string? metadata)
        => AddDomainEvent(new TaskAddedToChecklist(
            ChecklistId: Id,
            TaskId: Guid.NewGuid(),
            Name: name,
            TaskType: taskType,
            Planned: planned,
            Metadata: metadata,
            CreatedAt: DateTime.UtcNow
        ));

    public void StartTask(Guid taskId)
    {
        if (TryGetTask(taskId).IsCompleted)
            throw new DomainRuleViolation("Can't start already completed Task");

        AddDomainEvent(new TaskStarted(Id, taskId, DateTime.UtcNow));
    }

    public void CompleteTask(Guid taskId)
    {
        var task = TryGetTask(taskId);

        if (task.IsCompleted)
            throw new DomainRuleViolation("Can't complete already completed Task");

        if (task.ActualSchedule is null)
            throw new DomainRuleViolation("Can't complete not yet started Task");

        AddDomainEvent(new TaskCompleted(Id, taskId, DateTime.UtcNow));
    }

    public void RemoveTask(Guid taskId)
    {
        TryGetTask(taskId);
        AddDomainEvent(new TaskRemovedFromChecklist(Id, taskId));
    }

    public void SetUserRating(Rating rating)
        => AddDomainEvent(new UserRatingSet(Id, rating));

    public void SetLLMRating(Rating rating)
        => AddDomainEvent(new LLMRatingSet(Id, rating));

    // ---------- APPLY (STATE TRANSITIONS) ----------

    public void Apply(ChecklistCreated e)
    {
        UserId = e.UserId;
        Statistics = new Statistics(e.CreatedAt);
    }

    public void Apply(TaskAddedToChecklist e)
    {
        _tasks.Add(new Task(
            e.TaskId,
            e.Name,
            e.TaskType,
            e.Planned,
            e.ChecklistId,
            e.Metadata
        ));
    }

    public void Apply(TaskStarted e)
        => TryGetTask(e.TaskId).StartInternal();

    public void Apply(TaskCompleted e)
        => TryGetTask(e.TaskId).CompleteInternal();

    public void Apply(TaskRemovedFromChecklist e)
        => _tasks.Remove(TryGetTask(e.TaskId));

    public void Apply(UserRatingSet e)
        => Statistics = Statistics.WithUserRating(e.UserRating);

    public void Apply(LLMRatingSet e)
        => Statistics = Statistics.WithLLMRating(e.LLMRating);


    // ---------- HELPERS ----------

    private Task TryGetTask(Guid id) =>
        _tasks.FirstOrDefault(t => t.Id == id)
        ?? throw new DomainArgumentException($"Task {id} not found");
}
