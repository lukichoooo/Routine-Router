using Domain.SeedWork;

namespace Domain.Entities.Days.Events
{
    public sealed record TaskStarted(
            Guid ChecklistId,
            Guid TaskId,
            DateTime Date
            ) : IDomainEvent;
}
