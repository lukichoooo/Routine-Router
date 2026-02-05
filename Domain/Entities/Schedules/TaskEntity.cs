using Domain.Common.Exceptions;
using Domain.Common.ValueObjects;
using Domain.Entities.Schedules.ValueObjects;

namespace Domain.Entities.Schedules;

public sealed class TaskEntity
{
    public TaskId Id { get; private set; }
    public Name Name { get; private set; }
    public TaskType TaskType { get; private set; }

    public string? Metadata { get; private set; }

    public Schedule PlannedSchedule { get; private set; }
    public Schedule? ActualSchedule { get; private set; }

    public ChecklistId ChecklistId { get; private set; }


    public TaskEntity(
        TaskId id,
        Name name,
        TaskType taskType,
        Schedule planned,
        ChecklistId checklistId,
        string? metadata = null)
    {
        Id = id;
        Name = name ?? throw new DomainArgumentNullException(nameof(name));
        TaskType = taskType ?? throw new DomainArgumentNullException(nameof(taskType));
        PlannedSchedule = planned ?? throw new DomainArgumentNullException(nameof(planned));
        ChecklistId = checklistId;
        Metadata = metadata;
    }


    internal void Start(DateTimeOffset startTime)
        => ActualSchedule = new Schedule(startTime);

    internal void Complete(DateTimeOffset completionTime)
    {
        ActualSchedule = new Schedule(ActualSchedule!.StartTime, completionTime);
    }

    internal void UpdateMetadataInternal(string metadata)
        => Metadata = metadata;

    public bool IsCompleted()
        => ActualSchedule?.EndTime is not null;

    public bool IsStarted()
        => ActualSchedule?.StartTime is not null;

#pragma warning disable CS8618 
    private TaskEntity() { }
#pragma warning restore CS8618
}

