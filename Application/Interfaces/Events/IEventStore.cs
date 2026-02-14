using Domain.SeedWork;

namespace Application.Interfaces.Events;

public interface IEventStore
{
    Task Append(
        AggregateRootId aggregateId,
        IReadOnlyList<IDomainEvent> events,
        int? expectedVersion,
        CancellationToken ct);

    Task<List<IDomainEvent>> Load(
        AggregateRootId aggregateId,
        CancellationToken ct,
        int fromVersion = 0,
        int? toVersion = null);
}

