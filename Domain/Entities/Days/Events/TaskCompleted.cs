using Domain.SeedWork;

namespace Domain.Entities.Days.Events
{
    public sealed record TaskCompleted(
            Guid TaskId,
            TimeSpan Delay) : IDomainEvent;
}
