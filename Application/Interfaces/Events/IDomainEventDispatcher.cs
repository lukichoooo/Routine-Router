using Domain.SeedWork;

namespace Application.Interfaces.Events;


public interface IDomainEventDispatcher
{
    Task DispatchAsync(
            BaseDomainEvent<AggregateRootId> domainEvent,
            CancellationToken ct);
}

