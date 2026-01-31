using Domain.Entities.Schedules;

namespace Application.UseCases.Schedules
{
    public interface IChecklistRepo // TODO: Implement
    {
        Task<Checklist?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<Checklist?> GetForDayAsync(Guid userId, DateOnly date, CancellationToken ct);
        Task AddAsync(Checklist checklist, CancellationToken ct);
        Task SaveAsync(Checklist checklist, CancellationToken ct);
    }
}
