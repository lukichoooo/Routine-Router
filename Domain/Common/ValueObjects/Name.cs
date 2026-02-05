using Domain.Common.Exceptions;
using Domain.SeedWork;

namespace Domain.Common.ValueObjects;


public class Name : ValueObject
{
    public string Value { get; }

    public Name(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainArgumentNullException(nameof(value));

        Value = value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }



#pragma warning disable CS8618
    private Name() { }
#pragma warning restore CS8618
}

