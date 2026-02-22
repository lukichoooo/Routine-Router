using Application.Common.Exceptions;
using Application.Interfaces.Data;
using Application.Seedwork;
using Application.UseCases.Identity;
using Domain.Common.ValueObjects;
using Domain.Entities.Schedules.ValueObjects;

namespace Application.UseCases.Schedules.Commands;

public sealed record AddTaskToChecklistCommand(
        ChecklistId ChecklistId,
        Name Name,
        TaskType TaskType,
        Schedule Planned,
        string? Metadata
        ) : ICommand<TaskId>;


public class AddTaskToChecklistCommandHandler(
        IIdentityProvider identity,
        IChecklistRepo checklistRepo,
        IUserRepo userRepo,
        IUnitOfWork uow)
        : BaseCommandHandler<AddTaskToChecklistCommand, TaskId>(uow)
{
    protected override async Task<TaskId> Execute(
            AddTaskToChecklistCommand command,
            CancellationToken ct)
    {
        var userId = identity.GetCurrentUserId();
        if (await userRepo.GetById(userId, ct) is null)
            throw new ApplicationArgumentException($"User not found with Id={userId}");

        var checklist = await checklistRepo.GetById(command.ChecklistId, ct)
            ?? throw new ApplicationArgumentException($"Checklist not found with Id={command.ChecklistId}");

        if (checklist.UserId != userId)
            throw new ApplicationArgumentException($"User has no Checklist with ChecklistId={command.ChecklistId}");

        var taskId = checklist.AddTask(
                command.Name,
                command.TaskType,
                command.Planned,
                command.Metadata);

        await checklistRepo.Save(checklist, ct);

        return taskId;
    }
}


