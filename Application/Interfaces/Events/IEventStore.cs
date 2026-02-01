using Domain.SeedWork;

namespace Application.Interfaces.Events;

public interface IEventStore
{
    Task AppendAsync(
        AggregateRootId aggregateId,
        IReadOnlyCollection<IDomainEvent<AggregateRootId>> events,
        int? expectedVersion,
        CancellationToken ct);

    Task<IReadOnlyCollection<IDomainEvent<AggregateRootId>>> LoadAsync(
        AggregateRootId aggregateId,
        CancellationToken ct,
        int fromVersion = 0,
        int? toVersion = null);
}

