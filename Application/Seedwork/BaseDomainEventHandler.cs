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
        => Execute(evt, ct);


    protected abstract Task Execute(
            TE evt,
            CancellationToken ct);
}
