using Domain.SeedWork;

namespace Domain.Entities.Schedules.ValueObjects;


public class TaskId : EntityId
{
    public TaskId(Guid value) : base(value) { }

    private TaskId() { }
}


