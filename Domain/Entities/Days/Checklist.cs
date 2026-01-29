using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Common.ValueObjects;
using Domain.Entities.Days.Events;
using Domain.Entities.Days.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Days;

public sealed class Checklist : AggregateRoot
{
    private readonly ChecklistState State;

    // ---------- FACTORY ----------

    public Checklist(IEnumerable<IDomainEvent> events)
    {
        State = new ChecklistState();
        foreach (var e in events)
        {
            AppendEvent(e);
        }
    }

    private void AppendEvent(IDomainEvent e)
    {
        AddDomainEvent(e);
        ((dynamic)State).Apply((dynamic)e);
    }

    public void Create(Guid userId)
        => AppendEvent(new ChecklistCreated(
            ChecklistId: Guid.NewGuid(),
            UserId: userId,
            TimeStamp: Clock.Now
        ));

    // ---------- COMMANDS ----------

    public Guid AddTask(Name name, TaskType taskType, Schedule planned, string? metadata)
    {
        var taskId = Guid.NewGuid();
        AppendEvent(new TaskAddedToChecklist(
                    ChecklistId: State.Id,
                    TaskId: taskId,
                    Name: name,
                    TaskType: taskType,
                    Planned: planned,
                    Metadata: metadata,
                    TimeStamp: Clock.Now
                    ));
        return taskId;
    }

    public void StartTask(Guid taskId)
    {
        if (State.TryGetTask(taskId).IsCompleted)
            throw new DomainRuleViolation("Can't start already completed Task");

        AppendEvent(new TaskStarted(taskId, Clock.Now));
    }

    public void CompleteTask(Guid taskId)
    {
        var task = State.TryGetTask(taskId);

        if (task.IsCompleted)
            throw new DomainRuleViolation("Can't complete already completed Task");

        if (task.ActualSchedule is null)
            throw new DomainRuleViolation("Can't complete not yet started Task");

        AppendEvent(new TaskCompleted(State.Id, taskId, Clock.Now));
    }

    public void RemoveTask(Guid taskId)
    {
        State.TryGetTask(taskId);
        AppendEvent(new TaskRemovedFromChecklist(State.Id, taskId));
    }

    public void ChangeTaskMetadata(Guid taskId, string metadata)
    {
        if (metadata is null)
            throw new DomainArgumentNullException(nameof(metadata));
        State.TryGetTask(taskId);

        AppendEvent(new TaskUpdateMetadata(taskId, metadata));
    }

    public void SetUserRating(Rating rating)
        => AppendEvent(new UserRatingSet(State.Id, rating));

    public void SetLLMRating(Rating rating)
        => AppendEvent(new LLMRatingSet(State.Id, rating));


#pragma warning disable CS8618 
    private Checklist() { }
#pragma warning restore CS8618
}
