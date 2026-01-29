using Domain.Common.Exceptions;
using Domain.SeedWork;

namespace Domain.Common.ValueObjects
{
    public class Name : ValueObject
    {
        public string Value { get; }

        public Name(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainArgumentNullException(nameof(name));

            Value = name;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }



#pragma warning disable CS8618 
        private Name() { }
#pragma warning restore CS8618
    }
}
