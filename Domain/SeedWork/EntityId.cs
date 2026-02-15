namespace Domain.SeedWork;


// <summary>
// Base Class for all Aggregate Root Id
// inherits from ValueObject
// </summary>
public abstract class EntityId : ValueObject
{
    public Guid Value { get; }

    protected EntityId(Guid value) => Value = value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }


    public Guid ToGuid() => Value;

    public override bool Equals(object? obj)
        => obj is EntityId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    protected EntityId() { }
}

