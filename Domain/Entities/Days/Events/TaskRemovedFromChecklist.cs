using Domain.SeedWork;

namespace Domain.Entities.Days.Events
{
    public sealed record TaskRemovedFromChecklist(
            Guid ChecklistId,
            Guid TaskId) : IDomainEvent;
}
