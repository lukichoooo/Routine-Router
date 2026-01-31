using Application.Common.Seedwork;
using Application.Interfaces.Data;

namespace Application.UseCases.Schedules.Commands.CreateChecklist
{
    public class CreateChecklistCommandHandler
        : BaseCommandHandler<CreateChecklistCommand, Guid>
    {
        public CreateChecklistCommandHandler(IUnitOfWork uow) : base(uow)
        {
        }

        protected override Task<Guid> ExecuteAsync(CreateChecklistCommand command, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
