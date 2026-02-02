using Domain.SeedWork;

namespace Application.Common.Seedwork;

/// <summary>
/// Does Not Save Changes to Database Automatically
/// </summary>
public abstract class BaseDomainEventHandler
{
    public Task Handle(
            BaseDomainEvent<AggregateRootId> evt,
            CancellationToken ct)
        => ExecuteAsync(evt, ct);


    protected abstract Task ExecuteAsync(
            BaseDomainEvent<AggregateRootId> evt,
            CancellationToken ct);
}
