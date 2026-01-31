using Domain.SeedWork;

namespace Domain.Entities.Users.ValueObjects
{
    public class UserId : ValueObject
    {
        public Guid Value { get; }

        public UserId(Guid userId)
        {
            Value = userId;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
