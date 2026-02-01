using Domain.SeedWork;

namespace Application.Interfaces.Events;


public interface IDomainEventDispatcher
{
    Task DispatchAsync(
            IDomainEvent<AggregateRootId> domainEvent,
            CancellationToken ct);
}

