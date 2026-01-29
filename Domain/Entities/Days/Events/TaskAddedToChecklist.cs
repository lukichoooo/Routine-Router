using Domain.Common.ValueObjects;
using Domain.Entities.Days.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Days.Events
{
    public sealed record TaskAddedToChecklist
        (Guid ChecklistId,
         Guid TaskId,
         Name Name,
         TaskType TaskType,
         Schedule Planned,
         string? Metadata,
         DateTime CreatedAt
         ) : IDomainEvent;
}
