using Domain.Common.Exceptions;
using Domain.SeedWork;

namespace Domain.Entities.Users.ValueObjects
{
    public class PasswordHash : ValueObject
    {
        public string Value { get; }

        public PasswordHash(string value)
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
        private PasswordHash() { }
#pragma warning restore CS8618
    }
}
