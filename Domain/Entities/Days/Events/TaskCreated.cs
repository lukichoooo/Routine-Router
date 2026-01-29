using Domain.Common.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Days.Events
{
    public sealed record TaskCreated(
        Guid TaskId,
        Name Name,
        Schedule Planned,
        string? Metadata
        ) : IDomainEvent;
}
