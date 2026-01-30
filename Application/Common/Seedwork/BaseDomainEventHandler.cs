using Domain.SeedWork;
using MediatR;

namespace Application.Common.Seedwork;

/// <summary>
/// Does Not Save Changes to Database Automatically
/// </summary>
public abstract class BaseDomainEventHandler<TNotification>
    : INotificationHandler<TNotification> where TNotification : IDomainEvent
{
    public Task Handle(
            TNotification evt,
            CancellationToken ct)
        => ExecuteAsync(evt, ct);


    protected abstract Task ExecuteAsync(
            TNotification evt,
            CancellationToken ct);
}
