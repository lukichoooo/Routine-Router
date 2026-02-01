using Application.Interfaces.Events;
using Application.UseCases.Schedules;
using Domain.Entities.Schedules;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users.ValueObjects;

namespace Infrastructure.Repos;


public class ChecklistRepo : IChecklistRepo
{
    private readonly IEventStore _eventStore;

    public ChecklistRepo(IEventStore eventStore)
        => _eventStore = eventStore;

    public async Task AddAsync(Checklist checklist, CancellationToken ct)
        => await _eventStore.AppendAsync(
                checklist.Id,
                checklist.DomainEvents,
                checklist.Version,
                ct);

    public Task<Checklist?> GetByIdAsync(ChecklistId id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<Checklist?> GetForDayAsync(UserId userId, DateOnly date, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task SaveAsync(Checklist checklist, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}

