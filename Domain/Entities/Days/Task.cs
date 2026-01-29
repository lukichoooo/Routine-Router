using Domain.Common.Exceptions;
using Domain.Common.ValueObjects;
using Domain.Entities.Days.Events;
using Domain.Entities.Days.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Days;

public sealed class Task : Entity
{
    public Name Name { get; private set; }
    public string? Metadata { get; private set; }

    public Schedule PlannedSchedule { get; private set; }
    public Schedule? ActualSchedule { get; private set; }

    public Guid ChecklistId { get; private set; }

    public bool IsCompleted;


    public Task(
        Name name,
        Schedule planned,
        Guid ChecklistId,
        string? metadata = null)
    {
        Name = name ?? throw new DomainArgumentNullException(nameof(name));
        PlannedSchedule = planned ?? throw new DomainArgumentNullException(nameof(planned));
        this.ChecklistId = ChecklistId;
        Metadata = metadata;

        AddDomainEvent(new TaskCreated(Id, name, planned, metadata));
    }


    public void Start()
    {
        if (IsCompleted)
            throw new DomainException("Can't Start already completed Task");

        ActualSchedule = new Schedule(DateTimeOffset.Now);

        var delay = ActualSchedule.StartTime - PlannedSchedule.StartTime;

        AddDomainEvent(new TaskStarted(Id, delay));
    }

    public void Complete()
    {
        if (IsCompleted)
            throw new DomainException("Can't complete already completed Task");

        if (ActualSchedule is null)
            throw new DomainException("Can't complete unstarted Task");

        ActualSchedule = new Schedule(ActualSchedule.StartTime, DateTimeOffset.Now);
        IsCompleted = true;

        var delay = (TimeSpan)(ActualSchedule.EndTime - PlannedSchedule.EndTime)!;

        AddDomainEvent(new TaskCompleted(Id, delay));
    }

    public void UpdateMetadata(string? metadata)
    {
        Metadata = metadata ?? throw new DomainArgumentNullException(nameof(metadata));

        AddDomainEvent(new TaskMetadataUpdated(Id, metadata));
    }



#pragma warning disable CS8618 
    private Task() { } // EF
#pragma warning restore CS8618 
}

