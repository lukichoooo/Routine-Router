
using Application.Common.Exceptions;
using Application.Interfaces.Data;
using Application.Seedwork;
using Application.UseCases.Identity;
using Domain.Entities.Schedules.ValueObjects;

namespace Application.UseCases.Schedules.Commands;

public sealed record CompleteTaskCommand(
        ChecklistId ChecklistId,
        TaskId TaskId
        ) : ICommand<EmptyReturn>;


public class CompleteTaskCommandHandler(
        IIdentityProvider identity,
        IChecklistRepo checklistRepo,
        IUserRepo userRepo,
        IUnitOfWork uow)
        : BaseCommandHandler<CompleteTaskCommand, EmptyReturn>(uow)
{
    protected override async Task<EmptyReturn> Execute(
            CompleteTaskCommand command,
            CancellationToken ct)
    {
        var userId = identity.GetCurrentUserId();
        if (await userRepo.GetById(userId, ct) is null)
            throw new ApplicationArgumentException($"User not found with Id={userId}");

        var checklist = await checklistRepo.GetById(command.ChecklistId, ct)
            ?? throw new ApplicationArgumentException($"Checklist not found with Id={command.ChecklistId}");

        if (checklist.UserId != userId)
            throw new ApplicationArgumentException($"User has no Checklist with ChecklistId={command.ChecklistId}");

        checklist.CompleteTask(command.TaskId);

        await checklistRepo.Save(checklist, ct);

        return new EmptyReturn();
    }
}

