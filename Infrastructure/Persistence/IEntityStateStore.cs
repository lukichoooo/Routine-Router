using Domain.Entities.Schedules;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;
using Infrastructure.Persistence.Contexts;

namespace Infrastructure.Persistence;


public interface IEntityStateStore<TS, TID>
        where TS : State<TID>
        where TID : AggregateRootId
{
    Task<TS?> GetAsync(TID aggregateId, CancellationToken ct);

    Task AddAsync(TS aggregate, CancellationToken ct);
    void Update(TS aggregate);
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

    public async Task AddAsync(ChecklistState aggregate, CancellationToken ct)
        => await _context.Checklists.AddAsync(aggregate, ct);

    public void Update(ChecklistState aggregate)
        => _context.Checklists.Update(aggregate);
}


public class SQLiteUserStateStore : IEntityStateStore<UserState, UserId>
{
    private readonly EntitiesContext _context;

    public SQLiteUserStateStore(EntitiesContext context)
    {
        _context = context;
    }


    public async Task<UserState?> GetAsync(UserId aggregateId, CancellationToken ct)
        => await _context.Users.FindAsync(aggregateId.Value, ct);

    public async Task AddAsync(UserState aggregate, CancellationToken ct)
        => await _context.Users.AddAsync(aggregate, ct);

    public void Update(UserState aggregate)
        => _context.Users.Update(aggregate);
}

