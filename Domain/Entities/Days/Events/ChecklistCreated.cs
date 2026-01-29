using Domain.SeedWork;

namespace Domain.Entities.Days.Events
{
    public sealed record ChecklistCreated(Guid ChecklistId, Guid UserId) : IDomainEvent;
}
