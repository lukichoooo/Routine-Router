using Domain.SeedWork;

namespace Application.Interfaces.Events;


public interface IDomainEventDispatcher
{
    Task Dispatch(
            IDomainEvent domainEvent,
            CancellationToken ct);
}

