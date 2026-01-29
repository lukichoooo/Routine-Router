using Domain.SeedWork;

namespace Domain.Entities.Days.Events
{
    public sealed record TaskStarted(
            Guid TaskId,
            TimeSpan Delay
            ) : IDomainEvent;
}
