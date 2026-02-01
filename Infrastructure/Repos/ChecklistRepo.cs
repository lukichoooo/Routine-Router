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

    public ChecklistRepo(
            IEventStore eventStore,
            ITrackedEntities trackedEntities) : base(trackedEntities)
    {
        _eventStore = eventStore;
    }


    public Task<Checklist?> GetByIdAsync(ChecklistId id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<Checklist?> GetForDayAsync(UserId userId, DateOnly date, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    protected override async Task SaveAsyncProtected(Checklist aggregate, CancellationToken ct)
        => await _eventStore.AppendAsync(
                aggregate.Id,
                aggregate.DomainEvents,
                aggregate.Version,
                ct);
}

