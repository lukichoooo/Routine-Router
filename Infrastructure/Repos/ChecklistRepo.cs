using Application.Interfaces.Events;
using Application.UseCases.Schedules;
using Domain.Entities.Schedules;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users.ValueObjects;
using Infrastructure.Persistence;
using Infrastructure.Repos.Common;

namespace Infrastructure.Repos;


public class ChecklistRepo : BaseRepository<Checklist, ChecklistId, ChecklistState>, IChecklistRepo
{
    private readonly IEventStore _eventStore;

    public ChecklistRepo(
            IEventStore eventStore,
            IEntityStateStore<ChecklistState, ChecklistId> stateStore)
        : base(stateStore)
    {
        _eventStore = eventStore;
    }

    protected override Task SaveEventsAsync(Checklist aggregate, CancellationToken ct)
        => _eventStore.AppendAsync(
                aggregate.Id,
                aggregate.DomainEvents,
                expectedVersion: aggregate.StoredVersion,
                ct);

    // Queries

    public Task<Checklist?> GetForDayAsync(UserId userId, DateOnly date, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public override async Task<Checklist?> GetByIdAsync(ChecklistId aggregateId, CancellationToken ct)
    {
        var checklistState = await _stateStore.GetAsync(aggregateId, ct);
        if (checklistState is not null)
            return new Checklist(ref checklistState);

        var events = await _eventStore.LoadAsync(aggregateId, ct);
        if (events.Count == 0)
            return null;

        var checklist = new Checklist(events);
        await _stateStore.AddAsync(checklist.State, ct);

        return checklist;
    }

}

