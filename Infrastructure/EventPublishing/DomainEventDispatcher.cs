using Application.Interfaces.Events;
using Domain.SeedWork;
using MediatR;

namespace Infrastructure.EventPublishing;


public class DomainEventDispatcher(IPublisher publisher) : IDomainEventDispatcher
{
    public Task Dispatch(IDomainEvent domainEvent, CancellationToken ct)
    {
        return publisher.Publish(domainEvent, ct);
    }
}

