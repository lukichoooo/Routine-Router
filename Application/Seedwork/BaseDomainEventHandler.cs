using Domain.SeedWork;
using MediatR;

namespace Application.Seedwork;

/// <summary>
/// Does Not Save Changes to Database Automatically
/// </summary>
public abstract class BaseDomainEventHandler<TE> : INotificationHandler<TE>
where TE : IDomainEvent
{
    public Task Handle(
            TE evt,
            CancellationToken ct)
        => ExecuteAsync(evt, ct);


    protected abstract Task ExecuteAsync(
            TE evt,
            CancellationToken ct);
}
