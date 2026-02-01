using Domain.SeedWork;

namespace Domain.Entities.Schedules.ValueObjects;


public class TaskId : ValueObject
{
    public Guid Value { get; }

    public TaskId(Guid value)
    {
        Value = value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}


