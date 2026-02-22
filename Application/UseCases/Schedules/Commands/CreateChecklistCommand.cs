using Application.Common.Exceptions;
using Application.Interfaces.Data;
using Application.Seedwork;
using Application.UseCases.Identity;
using Domain.Entities.Schedules;
using Domain.Entities.Schedules.ValueObjects;

namespace Application.UseCases.Schedules.Commands;


public sealed record CreateChecklistCommand : ICommand<ChecklistId>;


public class CreateChecklistCommandHandler(
        IIdentityProvider identity,
        IChecklistRepo checklistRepo,
        IUserRepo userRepo,
        IUnitOfWork uow)
        : BaseCommandHandler<CreateChecklistCommand, ChecklistId>(uow)
{
    protected override async Task<ChecklistId> Execute(
            CreateChecklistCommand command,
            CancellationToken ct)
    {
        var userId = identity.GetCurrentUserId();
        if (await userRepo.GetById(userId, ct) is null)
            throw new ApplicationArgumentException($"User not found with Id={userId}");

        var checklist = new Checklist();
        var checklistId = new ChecklistId(Guid.NewGuid());

        checklist.Create(checklistId, userId);

        await checklistRepo.Save(checklist, ct);

        return checklistId;
    }
}


