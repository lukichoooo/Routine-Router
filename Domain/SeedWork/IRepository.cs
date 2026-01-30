namespace Domain.SeedWork;

public interface IRepository<T> where T : AggregateRoot
{
    Task<T> Load(Guid id, CancellationToken ct);
    Task Commit(T aggregate, CancellationToken ct);
}

