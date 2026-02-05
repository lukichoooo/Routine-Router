namespace Domain.SeedWork;


// <summary>
// Base Class for all Aggregate Root Id
// inherits from ValueObject
// </summary>
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


    public Guid ToGuid() => Value;

    public override bool Equals(object? obj)
        => obj is AggregateRootId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    protected AggregateRootId() { }
}

