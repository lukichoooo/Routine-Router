using Domain.Entities.Schedules;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;

namespace Application.UseCases.Schedules;


public interface IChecklistRepo : IRepository<Checklist, ChecklistId>
{
    Task<Checklist?> GetForDayAsync(UserId userId, DateOnly date, CancellationToken ct);
}

