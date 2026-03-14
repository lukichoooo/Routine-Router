using Domain.SeedWork;

namespace Application.Interfaces.Events;


public interface IDomainEventDispatcher
{
    ValueTask Dispatch(IDomainEvent domainEvent, CancellationToken ct);
}

