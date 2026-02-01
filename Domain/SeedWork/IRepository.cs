namespace Domain.SeedWork;


// <summary>
// Base for all repositories
// </summary>
public interface IRepository<TA, TID>
    where TA : AggregateRoot<TID>
    where TID : AggregateRootId
{
    Task SaveAsync(TA aggregate, int expectedVersion, CancellationToken ct);
    Task<TA?> GetByIdAsync(TID aggregateId, CancellationToken ct);
}
