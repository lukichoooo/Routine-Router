using Domain.SeedWork;

namespace Domain.Entities.Days.Events
{
    public sealed record TaskUpdateMetadata(
            Guid TaskId,
            string Metadata) : IDomainEvent;
}
