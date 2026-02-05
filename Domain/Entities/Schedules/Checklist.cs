using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Common.ValueObjects;
using Domain.Entities.Schedules.Events;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Schedules;

public sealed class Checklist : AggregateRoot<ChecklistId, ChecklistState>
{
    public Checklist(IEnumerable<IDomainEvent>? history = null)
        : base(history) { }

    public Checklist(ref ChecklistState state)
        : base(ref state) { }

    public UserId UserId => State.UserId;


    public void Create(ChecklistId id, UserId userId)
        => AppendEvent(new ChecklistCreated(
            AggregateId: id,
            Version: NextVersion,
            Timestamp: Clock.Now,
            UserId: userId));

    public TaskId AddTask(Name name, TaskType taskType, Schedule planned, string? metadata)
    {
        var taskId = new TaskId(Guid.NewGuid());
        AppendEvent(new TaskAddedToChecklist(
                    AggregateId: State.Id,
                    Version: NextVersion,
                    Timestamp: Clock.Now,
                    TaskId: taskId,
                    Name: name,
                    TaskType: taskType,
                    Planned: planned,
                    Metadata: metadata
                    ));
        return taskId;
    }

    public void StartTask(TaskId taskId)
    {
        if (State.TryGetTask(taskId).IsCompleted())
            throw new DomainRuleViolation("Can't start already completed Task");

        AppendEvent(new TaskStarted(
                    AggregateId: State.Id,
                    Version: NextVersion,
                    Timestamp: Clock.Now,
                    taskId));
    }

    public void CompleteTask(TaskId taskId)
    {
        var task = State.TryGetTask(taskId);

        if (task.IsCompleted())
            throw new DomainRuleViolation("Can't complete already completed Task");

        if (task.ActualSchedule?.StartTime is null)
            throw new DomainRuleViolation("Can't complete not yet started Task");

        AppendEvent(new TaskCompleted(
                    AggregateId: State.Id,
                    Version: NextVersion,
                    Timestamp: Clock.Now,
                    taskId
                    ));
    }

    public void RemoveTask(TaskId taskId)
    {
        State.TryGetTask(taskId);
        AppendEvent(new TaskRemovedFromChecklist(
                    AggregateId: State.Id,
                    Version: NextVersion,
                    Timestamp: Clock.Now,
                    taskId
                    ));
    }

    public void ChangeTaskMetadata(TaskId taskId, string metadata)
    {
        if (metadata is null)
            throw new DomainArgumentNullException(nameof(metadata));
        State.TryGetTask(taskId);

        AppendEvent(new TaskMetadataUpdated(
                    AggregateId: State.Id,
                    Version: NextVersion,
                    Timestamp: Clock.Now,
                    taskId,
                    metadata
                    ));
    }

    public void SetUserRating(Rating rating)
        => AppendEvent(new UserRatingSet(
                    AggregateId: State.Id,
                    Version: NextVersion,
                    Timestamp: Clock.Now,
                    rating));

    public void SetLLMRating(Rating rating)
        => AppendEvent(new LLMRatingSet(
                    AggregateId: State.Id,
                    Version: NextVersion,
                    Timestamp: Clock.Now,
                    rating));


    private Checklist() { }
}
