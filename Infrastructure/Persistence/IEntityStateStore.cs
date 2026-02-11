using Domain.Entities.Schedules;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

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


    public Task<ChecklistState?> GetAsync(ChecklistId aggregateId, CancellationToken ct)
        => _context.Checklists
            .FirstOrDefaultAsync(u => u.Id == aggregateId, ct);

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


    public Task<UserState?> GetAsync(UserId aggregateId, CancellationToken ct)
        => _context.Users
            .FirstOrDefaultAsync(u => u.Id == aggregateId, ct);

    public async Task AddAsync(UserState aggregateState, CancellationToken ct)
        => await _context.Users.AddAsync(aggregateState, ct);
}

