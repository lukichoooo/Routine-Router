namespace Domain.SeedWork;


// <summary>
// Base for all repositories
// </summary>
public interface IRepository<TA, TID>
    where TA : AggregateRoot<TID>
    where TID : EntityId
{
    Task Save(TA aggregate, CancellationToken ct);

    Task<TA?> GetById(TID aggregateId, CancellationToken ct);
}
