namespace Domain.SeedWork;


// <summary>
// Base for all repositories
// </summary>
public interface IRepository<TA, TID>
    where TA : AggregateRoot<TID>
    where TID : AggregateRootId
{
    Task AddAsync(TA aggregate, CancellationToken ct);

    Task<TA?> GetByIdAsync(TID aggregateId, CancellationToken ct);
}
