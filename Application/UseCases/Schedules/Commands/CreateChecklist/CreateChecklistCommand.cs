using Application.Seedwork;
using Domain.Entities.Schedules.ValueObjects;

namespace Application.UseCases.Schedules.Commands.CreateChecklist
{
    public sealed record CreateChecklistCommand : ICommand<ChecklistId>;
}
