namespace Domain.SeedWork;


public abstract class AggregateRootId : ValueObject
{
    public Guid Value { get; }

    public AggregateRootId(Guid value)
    {
        Value = value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}

