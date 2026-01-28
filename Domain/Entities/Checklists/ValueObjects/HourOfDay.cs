using Domain.SeedWork;

namespace Domain.Entities.Checklists.ValueObjects;

public class HourOfDay : ValueObject
{
    public double Value { get; }

    public HourOfDay(int hour)
    {
        if (hour < 0 || hour > 24)
            throw new DomainException("Hour of day must be between 0 and 23");

        Value = hour;
    }

    public override string ToString() => Value.ToString("D2") + ":00";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

