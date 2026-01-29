using Domain.SeedWork;

namespace Domain.Entities.Days.Events
{
    public sealed record TaskCompleted(
            Guid ChecklistId,
            Guid TaskId,
            DateTimeOffset TimeStamp) : IDomainEvent;
}
