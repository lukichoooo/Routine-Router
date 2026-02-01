namespace Domain.SeedWork;

public interface IRepository<TAggregateRootId>
    where TAggregateRootId : AggregateRootId
{
    Task<AggregateRoot<TAggregateRootId>> LoadAsync(AggregateRootId id, CancellationToken ct);
    Task SaveAsync(AggregateRoot<TAggregateRootId> aggregate, CancellationToken ct);
}

