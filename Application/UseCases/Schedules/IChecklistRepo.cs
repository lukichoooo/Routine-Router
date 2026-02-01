using Domain.Entities.Schedules;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users.ValueObjects;

namespace Application.UseCases.Schedules;


public interface IChecklistRepo
{
    Task<Checklist?> GetByIdAsync(ChecklistId id, CancellationToken ct);
    Task<Checklist?> GetForDayAsync(UserId userId, DateOnly date, CancellationToken ct);
    Task SaveAsync(Checklist checklist, CancellationToken ct);
}

