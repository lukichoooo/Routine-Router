using Domain.SeedWork;

namespace Domain.Common.ValueObjects
{
    public class Name : ValueObject
    {
        public string Value { get; } = null!;

        public Name(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name cannot be empty");

            Value = name;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }



        private Name() { }
    }
}
