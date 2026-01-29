using Domain.SeedWork;

namespace Domain.Entities.Days.Events
{
    public sealed record TaskAddedToChecklist
        (Guid ChecklistId,
         Guid TaskId
         ) : IDomainEvent;
}
