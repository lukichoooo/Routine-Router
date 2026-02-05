using Application.Common.Exceptions;
using Application.Interfaces.Data;
using Application.Seedwork;
using Application.UseCases.Identity;
using Domain.Entities.Schedules;
using Domain.Entities.Schedules.ValueObjects;

namespace Application.UseCases.Schedules.Commands;


public sealed record CreateChecklistCommand : ICommand<ChecklistId>;


public class CreateChecklistCommandHandler
    : BaseCommandHandler<CreateChecklistCommand, ChecklistId>
{
    private readonly IIdentityProvider _identity;
    private readonly IChecklistRepo _checklistRepo;
    private readonly IUserRepo _userRepo;

    public CreateChecklistCommandHandler(
            IIdentityProvider identity,
            IChecklistRepo checklistRepo,
            IUserRepo userRepo,
            IUnitOfWork uow) : base(uow)
    {
        _identity = identity;
        _checklistRepo = checklistRepo;
        _userRepo = userRepo;
    }

    protected override async Task<ChecklistId> ExecuteAsync(
            CreateChecklistCommand command,
            CancellationToken ct)
    {
        var userId = _identity.GetCurrentUserId();

        var checklist = new Checklist();
        var checklistId = new ChecklistId(Guid.NewGuid());

        if (await _userRepo.GetByIdAsync(userId, ct) is null)
            throw new ApplicationArgumentException($"User not found with Id={userId}");

        checklist.Create(checklistId, userId);

        await _checklistRepo.AddAsync(checklist, ct);

        return checklistId;
    }
}


