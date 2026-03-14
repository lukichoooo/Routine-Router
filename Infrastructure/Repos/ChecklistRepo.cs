#pragma warning disable CS9107

using Application.Interfaces.Events;
using Application.UseCases.Schedules;
using Domain.Entities.Schedules;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users.ValueObjects;
using Infrastructure.Persistence;
using Infrastructure.Repos.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos;


public class ChecklistRepo
(IEventStore eventStore, IEntityStateStore<ChecklistState, ChecklistId> stateStore)
    : BaseRepository<Checklist, ChecklistId, ChecklistState>(stateStore, eventStore), IChecklistRepo
{
    public async Task<IEnumerable<Checklist>> GetForDay(UserId userId, DateOnly date, CancellationToken ct)
        => (await stateStore.Query
            .Where(c => c.UserId == userId && c.Statistics.GetDate() == date)
            .ToListAsync(ct))
            .ConvertAll(Checklist.Create);
}


#pragma warning restore CS9107
