using Domain.Common;
using Domain.SeedWork;

namespace Domain.Entities.Schedules.ValueObjects;


public sealed class Schedule : ValueObject
{
    public DateTimeOffset StartTime { get; }
    public DateTimeOffset? EndTime { get; }

    public Schedule(DateTimeOffset startTime, DateTimeOffset? endTime = null)
    {
        StartTime = startTime;
        EndTime = endTime;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return StartTime;
        yield return EndTime;
    }


    public TimeSpan GetDuration()
        => EndTime?.Subtract(StartTime) ?? TimeSpan.Zero;

    public TimeSpan GetTimeFromStart()
        => Clock.CurrentTime.Subtract(StartTime);


    private Schedule() { }
}



