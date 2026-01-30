using Domain.SeedWork;

namespace Application.Interfaces.Events;

public interface IEventStore
{
    Task AppendAsync(
        Guid aggregateId,
        IReadOnlyCollection<IDomainEvent> events,
        int expectedVersion,
        CancellationToken ct);
}

