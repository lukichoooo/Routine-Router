using Domain.SeedWork;

namespace Application.Interfaces.Events;

public interface IEventStore
{
    Task AppendAsync<TAggregateId>(
        TAggregateId aggregateId,
        IReadOnlyCollection<IDomainEvent<TAggregateId>> events,
        int expectedVersion,
        CancellationToken ct);

    Task<IReadOnlyCollection<IDomainEvent<TAggregateId>>> LoadAsync<TAggregateId>(
        TAggregateId aggregateId,
        CancellationToken ct);

    Task<IReadOnlyCollection<IDomainEvent<TAggregateId>>> LoadAsync<TAggregateId>(
        TAggregateId aggregateId,
        int fromVersion,
        CancellationToken ct);
}

