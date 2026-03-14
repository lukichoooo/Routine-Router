using Domain.SeedWork;
using Mediator;

namespace Application.Seedwork;

/// <summary>
/// Does Not Save Changes to Database Automatically
/// </summary>
public abstract class BaseDomainEventHandler<TEvent> : INotificationHandler<TEvent>
where TEvent : IDomainEvent
{
    public ValueTask Handle(
            TEvent evt,
            CancellationToken ct)
        => Execute(evt, ct);


    protected abstract ValueTask Execute(
            TEvent evt,
            CancellationToken ct);
}
