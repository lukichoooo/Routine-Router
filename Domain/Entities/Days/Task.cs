using Domain.Common.Exceptions;
using Domain.Common.ValueObjects;
using Domain.Entities.Days.Events;
using Domain.SeedWork;

namespace Domain.Entities.Days;

public sealed class Task : Entity
{
    public Name Name { get; private set; }
    public string? Metadata { get; private set; }

    public Schedule Planned { get; private set; }
    public Schedule? Actual { get; private set; }

    public bool IsCompleted;


    public Task(
        Name name,
        Schedule planned,
        string? metadata = null)
    {
        Name = name ?? throw new DomainArgumentNullException(nameof(name));
        Planned = planned ?? throw new DomainArgumentNullException(nameof(planned));
        Metadata = metadata;

        AddDomainEvent(new TaskCreated(Id, name, planned, metadata));
    }


    public void Start()
    {
        if (IsCompleted)
            throw new DomainException("Can't Start already completed Task");

        Actual = new Schedule(DateTimeOffset.Now);

        var delay = Actual.StartTime - Planned.StartTime;

        AddDomainEvent(new TaskStarted(Id, delay));
    }

    public void Complete()
    {
        if (IsCompleted)
            throw new DomainException("Can't complete already completed Task");

        if (Actual is null)
            throw new DomainException("Can't complete unstarted Task");

        Actual = new Schedule(Actual.StartTime, DateTimeOffset.Now);
        IsCompleted = true;

        var delay = (TimeSpan)(Actual.EndTime - Planned.EndTime)!;

        AddDomainEvent(new TaskCompleted(Id, delay));
    }

    public void UpdateMetadata(string? metadata)
        => Metadata = metadata ?? throw new DomainArgumentNullException(nameof(metadata));



#pragma warning disable CS8618 
    private Task() { } // EF
#pragma warning restore CS8618 
}

