using Domain.Entities.Schedules;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;
using Infrastructure.Persistence.Contexts;

namespace Infrastructure.Persistence;


// <summary>
// does NOT call SaveChanges()
// only saves most recent version of each entity
// </summary>
public interface IEntityStateStore<TS, TID>
        where TS : AggregateRootState<TID>
        where TID : AggregateRootId
{
    Task<TS?> GetAsync(TID aggregateId, CancellationToken ct);

    Task AddAsync(TS aggregate, CancellationToken ct);
}


public class SQLiteChecklistStateStore : IEntityStateStore<ChecklistState, ChecklistId>
{
    private readonly EntitiesContext _context;

    public SQLiteChecklistStateStore(EntitiesContext context)
    {
        _context = context;
    }


    public async Task<ChecklistState?> GetAsync(ChecklistId aggregateId, CancellationToken ct)
        => await _context.Checklists.FindAsync(aggregateId, ct);

    public async Task AddAsync(ChecklistState aggregateState, CancellationToken ct)
        => await _context.Checklists.AddAsync(aggregateState, ct);
}


public class SQLiteUserStateStore : IEntityStateStore<UserState, UserId>
{
    private readonly EntitiesContext _context;

    public SQLiteUserStateStore(EntitiesContext context)
    {
        _context = context;
    }


    public async Task<UserState?> GetAsync(UserId aggregateId, CancellationToken ct)
        => await _context.Users.FindAsync(aggregateId, ct);

    public async Task AddAsync(UserState aggregateState, CancellationToken ct)
        => await _context.Users.AddAsync(aggregateState, ct);
}

