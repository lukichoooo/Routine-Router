using Domain.SeedWork;

namespace Domain.Entities.Days.Events
{
    public sealed record TaskMetadataUpdated(
            Guid TaskId,
            string Metadata) : IDomainEvent;
}
