using Application.Interfaces.Events;
using Application.UseCases.Schedules;
using Domain.Entities.Schedules;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users.ValueObjects;
using Infrastructure.Persistence;
using Infrastructure.Repos.Common;

namespace Infrastructure.Repos;


public class ChecklistRepo : BaseRepository<Checklist, ChecklistId>, IChecklistRepo
{
    private readonly IEventStore _eventStore;
    private readonly IEntityStateStore<ChecklistState, ChecklistId> _stateStore;

    public ChecklistRepo(
            IEventStore eventStore,
            IEntityStateStore<ChecklistState, ChecklistId> stateStore,
            ITrackedEntities trackedEntities)
        : base(trackedEntities)
    {
        _eventStore = eventStore;
        _stateStore = stateStore;
    }


    public override async Task<Checklist?> GetByIdAsync(ChecklistId aggregateId, CancellationToken ct)
    {
        var checklistState = await _stateStore.GetAsync(aggregateId, ct);
        if (checklistState is not null)
            return new Checklist(checklistState, checklistState.Version);

        var events = await _eventStore.LoadAsync(aggregateId, ct);
        if (events.Count == 0)
            return null;

        return new Checklist(events);
    }

    protected override async Task SaveAsyncProtected(Checklist aggregate, CancellationToken ct)
        => await _eventStore.AppendAsync(
                aggregate.Id,
                aggregate.DomainEvents,
                expectedVersion: aggregate.StoredVersion,
                ct);


    public Task<Checklist?> GetForDayAsync(UserId userId, DateOnly date, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}

