using Domain.Entities.Schedules;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;


// <summary>
// only saves most recent version of each entity
// uses batching
// </summary>
public interface IEntityStateStore<TState, TId>
        where TState : AggregateRootState<TId>
        where TId : EntityId
{
    Task<TState?> Get(TId aggregateId, CancellationToken ct);

    Task Save(TState aggregateState, CancellationToken ct);

    void Update(TState aggregateState);
}


public class SQLiteChecklistStateStore(StateContext context) : IEntityStateStore<ChecklistState, ChecklistId>
{
    public async Task<ChecklistState?> Get(ChecklistId aggregateId, CancellationToken ct)
        => await context.Checklists
        .Include(c => c.Tasks
                .OrderBy(t => t.PlannedSchedule.StartTime))
        .FirstOrDefaultAsync(c => c.Id == aggregateId, ct);

    public async Task Save(ChecklistState aggregateState, CancellationToken ct)
        => await context.AddAsync(aggregateState, ct);

    public void Update(ChecklistState aggregateState)
        => context.Update(aggregateState);
}


public class SQLiteUserStateStore(StateContext context) : IEntityStateStore<UserState, UserId>
{
    public async Task<UserState?> Get(UserId aggregateId, CancellationToken ct)
        => await context.Users
        .FirstOrDefaultAsync(c => c.Id == aggregateId, ct);

    public async Task Save(UserState aggregateState, CancellationToken ct)
        => await context.AddAsync(aggregateState, ct);

    public void Update(UserState aggregateState)
        => context.Update(aggregateState);
}

