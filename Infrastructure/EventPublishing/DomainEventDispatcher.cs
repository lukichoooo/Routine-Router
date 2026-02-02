using Application.Interfaces.Events;
using Domain.SeedWork;
using MediatR;

namespace Infrastructure.EventPublishing;


public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IPublisher _publisher;

    public DomainEventDispatcher(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public Task DispatchAsync(IDomainEvent domainEvent, CancellationToken ct)
    {
        return _publisher.Publish(domainEvent, ct);
    }
}

