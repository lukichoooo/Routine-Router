using Domain.SeedWork;

namespace Domain.Entities.Schedules.ValueObjects;


public class ChecklistId : AggregateRootId
{
    public ChecklistId(Guid value) : base(value) { }

    public ChecklistId() { }
}

