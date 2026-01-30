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

    public Checklist(IEnumerable<IDomainEvent>? events = null)
    {
        State = new ChecklistState();
        foreach (var e in events ?? [])
        {
            ((dynamic)State).Apply((dynamic)e);
            Version = e.Version;
        }
    }

    private void AppendEvent(IDomainEvent e)
    {
        AddDomainEvent(e);
        ((dynamic)State).Apply((dynamic)e);
        Version = e.Version;
    }

    // ---------- COMMANDS ----------

    public void Create(Guid userId)
        => AppendEvent(new ChecklistCreated(
            AggregateId: Guid.NewGuid(),
            Version: GetNextVersion,
            Timestamp: Clock.Now,
            UserId: userId));

    public Guid AddTask(Name name, TaskType taskType, Schedule planned, string? metadata)
    {
        var taskId = Guid.NewGuid();
        AppendEvent(new TaskAddedToChecklist(
                    AggregateId: State.Id,
                    Version: GetNextVersion,
                    Timestamp: Clock.Now,
                    TaskId: taskId,
                    Name: name,
                    TaskType: taskType,
                    Planned: planned,
                    Metadata: metadata
                    ));
        return taskId;
    }

    public void StartTask(Guid taskId)
    {
        if (State.TryGetTask(taskId).IsCompleted)
            throw new DomainRuleViolation("Can't start already completed Task");

        AppendEvent(new TaskStarted(
                    AggregateId: State.Id,
                    Version: GetNextVersion,
                    Timestamp: Clock.Now,
                    taskId));
    }

    public void CompleteTask(Guid taskId)
    {
        var task = State.TryGetTask(taskId);

        if (task.IsCompleted)
            throw new DomainRuleViolation("Can't complete already completed Task");

        if (task.ActualSchedule is null)
            throw new DomainRuleViolation("Can't complete not yet started Task");

        AppendEvent(new TaskCompleted(
                    AggregateId: State.Id,
                    Version: GetNextVersion,
                    Timestamp: Clock.Now,
                    taskId
                    ));
    }

    public void RemoveTask(Guid taskId)
    {
        State.TryGetTask(taskId);
        AppendEvent(new TaskRemovedFromChecklist(
                    AggregateId: State.Id,
                    Version: GetNextVersion,
                    Timestamp: Clock.Now,
                    taskId
                    ));
    }

    public void ChangeTaskMetadata(Guid taskId, string metadata)
    {
        if (metadata is null)
            throw new DomainArgumentNullException(nameof(metadata));
        State.TryGetTask(taskId);

        AppendEvent(new TaskUpdateMetadata(
                    AggregateId: State.Id,
                    Version: GetNextVersion,
                    Timestamp: Clock.Now,
                    taskId,
                    metadata
                    ));
    }

    public void SetUserRating(Rating rating)
        => AppendEvent(new UserRatingSet(
                    AggregateId: State.Id,
                    Version: GetNextVersion,
                    Timestamp: Clock.Now,
                    rating));

    public void SetLLMRating(Rating rating)
        => AppendEvent(new LLMRatingSet(
                    AggregateId: State.Id,
                    Version: GetNextVersion,
                    Timestamp: Clock.Now,
                    rating));


#pragma warning disable CS8618 
    private Checklist() { }
#pragma warning restore CS8618
}
