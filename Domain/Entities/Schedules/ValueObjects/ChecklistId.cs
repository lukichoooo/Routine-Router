using Domain.SeedWork;

namespace Domain.Entities.Schedules.ValueObjects;


public class ChecklistId : ValueObject, IAggregateRootId
{
    public Guid Value { get; }

    public ChecklistId(Guid checklistId)
    {
        Value = checklistId;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}

