namespace Domain.SeedWork;

public interface IRepository
{
    Task<AggregateRoot<IAggregateRootId>> LoadAsync(IAggregateRootId id, CancellationToken ct);
    Task SaveAsync(AggregateRoot<IAggregateRootId> aggregate, CancellationToken ct);
}

