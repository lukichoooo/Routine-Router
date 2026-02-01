using Application.Common.Seedwork;
using Application.Interfaces.Data;
using Application.UseCases.Identity;
using Domain.Entities.Schedules;
using Domain.Entities.Schedules.ValueObjects;

namespace Application.UseCases.Schedules.Commands.CreateChecklist;


// TODO: write tests
public class CreateChecklistCommandHandler
    : BaseCommandHandler<CreateChecklistCommand, ChecklistId>
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

    protected override async Task<ChecklistId> ExecuteAsync(CreateChecklistCommand command, CancellationToken ct)
    {
        var userId = _identity.GetCurrentUserId();
        var checklist = new Checklist();
        var checklistId = new ChecklistId(Guid.NewGuid());

        // TODO
        var expectedVersion = checklist.Version;

        checklist.Create(checklistId, userId);

        await _repo.SaveAsync(checklist, expectedVersion, ct);

        return checklistId;
    }
}

