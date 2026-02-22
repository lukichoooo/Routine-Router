using Domain.SeedWork;

namespace Domain.Entities.Schedules.ValueObjects;


public class ChecklistId : EntityId
{
    public ChecklistId(Guid value) : base(value) { }

    private ChecklistId() { }
}

