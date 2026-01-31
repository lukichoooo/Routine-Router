using Application.Common.Seedwork;

namespace Application.UseCases.Schedules.Commands.CreateChecklist
{
    public sealed record CreateChecklistCommand : ICommand<Guid>;
}
