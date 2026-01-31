namespace Domain.SeedWork;

public interface IRepository<T, TAggregateId> where T : AggregateRoot<TAggregateId>
{
    Task<T> LoadAsync(Guid id, CancellationToken ct);
    Task SaveAsync(T aggregate, CancellationToken ct);
}

