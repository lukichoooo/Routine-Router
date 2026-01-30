using Domain.Common.Exceptions;
using Domain.Common.ValueObjects;
using Domain.Entities.Schedules.ValueObjects;

namespace Domain.Entities.Schedules;

public sealed class TaskEntity
{
    public Guid Id { get; private set; }
    public Name Name { get; private set; }
    public TaskType TaskType { get; private set; }

    public string? Metadata { get; private set; }

    public Schedule PlannedSchedule { get; private set; }
    public Schedule? ActualSchedule { get; private set; }

    public Guid ChecklistId { get; private set; }


    public TaskEntity(
        Guid id,
        Name name,
        TaskType taskType,
        Schedule planned,
        Guid checklistId,
        string? metadata = null)
    {
        Id = id;
        Name = name ?? throw new DomainArgumentNullException(nameof(name));
        TaskType = taskType ?? throw new DomainArgumentNullException(nameof(taskType));
        PlannedSchedule = planned ?? throw new DomainArgumentNullException(nameof(planned));
        ChecklistId = checklistId;
        Metadata = metadata;
    }


    internal void StartInternal(DateTimeOffset startTime)
        => ActualSchedule = new Schedule(startTime);

    internal void CompleteInternal(DateTimeOffset completionTime)
    {
        ActualSchedule = new Schedule(ActualSchedule!.StartTime, completionTime);
    }

    internal void UpdateMetadataInternal(string metadata)
        => Metadata = metadata;

    public bool IsCompleted()
        => ActualSchedule?.EndTime is not null;

    public bool IsStarted()
        => ActualSchedule?.StartTime is not null;
}

