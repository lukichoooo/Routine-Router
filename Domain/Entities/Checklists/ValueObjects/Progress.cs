using Domain.SeedWork;

namespace Domain.Entities.Checklists.ValueObjects;

public sealed class Progress : ValueObject
{
    public TimeSpan? AttemptDuration { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool Attempted { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AttemptDuration!;
        yield return IsCompleted;
        yield return Attempted;
    }

    private Progress() { }
}

