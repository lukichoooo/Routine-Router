using Application.Interfaces.Events;
using Domain.SeedWork;
using Mediator;

namespace Infrastructure.EventPublishing;


public class DomainEventDispatcher(IPublisher publisher) : IDomainEventDispatcher
{
    public ValueTask Dispatch(IDomainEvent domainEvent, CancellationToken ct)
    {
        return publisher.Publish(domainEvent, ct);
    }
}

