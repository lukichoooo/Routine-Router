using Domain.SeedWork;

namespace Application.Interfaces.Events;

public interface IEventStore<TAggregateId>
where TAggregateId : IAggregateRootId
{
    Task AppendAsync(
        TAggregateId aggregateId,
        IReadOnlyCollection<IDomainEvent<TAggregateId>> events,
        int expectedVersion,
        CancellationToken ct);

    Task<IReadOnlyCollection<IDomainEvent<TAggregateId>>> LoadAsync(
        TAggregateId aggregateId,
        CancellationToken ct);

    Task<IReadOnlyCollection<IDomainEvent<TAggregateId>>> LoadAsync(
        TAggregateId aggregateId,
        int fromVersion,
        CancellationToken ct);
}

