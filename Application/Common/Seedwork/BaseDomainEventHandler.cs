using Domain.SeedWork;

namespace Application.Common.Seedwork;

/// <summary>
/// Does Not Save Changes to Database Automatically
/// </summary>
public abstract class BaseDomainEventHandler
{
    public Task Handle(
            IDomainEvent<AggregateRootId> evt,
            CancellationToken ct)
        => ExecuteAsync(evt, ct);


    protected abstract Task ExecuteAsync(
            IDomainEvent<AggregateRootId> evt,
            CancellationToken ct);
}
