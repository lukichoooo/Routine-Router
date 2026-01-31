using Application.Common.Seedwork;
using Application.Interfaces.Data;
using Application.UseCases.Identity;
using Domain.Entities.Schedules;

namespace Application.UseCases.Schedules.Commands.CreateChecklist
{
    public class CreateChecklistCommandHandler
        : BaseCommandHandler<CreateChecklistCommand, Guid>
    {
        private readonly IIdentityProvider _identity;
        private readonly IChecklistRepo _repo;

        public CreateChecklistCommandHandler(
                IIdentityProvider identity,
                IChecklistRepo repo,
                IUnitOfWork uow) : base(uow)
        {
            _identity = identity;
            _repo = repo;
        }

        protected override async Task<Guid> ExecuteAsync(CreateChecklistCommand command, CancellationToken ct)
        {
            var checklist = new Checklist();
            var checklistId = Guid.NewGuid();
            var userId = _identity.GetCurrentUserId();

            checklist.Create(checklistId, userId);
            await _repo.AddAsync(checklist, ct);

            return checklistId;
        }
    }
}
