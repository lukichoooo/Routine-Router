using Application.Common.Exceptions;
using Application.Seedwork;
using Application.UseCases.Identity;
using Domain.Entities.Schedules;
using Domain.Entities.Schedules.ValueObjects;

namespace Application.UseCases.Schedules.Queries;


public sealed record GetChecklistByIdQuery(ChecklistId ChecklistId) : IQuery<ChecklistState>;

public class GetChecklistByIdQueryHandler(
        IIdentityProvider identity,
        IChecklistRepo checklistRepo,
        IUserRepo userRepo)
: BaseQueryHandler<GetChecklistByIdQuery, ChecklistState>
{
    protected override async Task<ChecklistState> Execute(
            GetChecklistByIdQuery query,
            CancellationToken ct)
    {
        var userId = identity.GetCurrentUserId();
        if (await userRepo.GetById(userId, ct) is null)
            throw new ApplicationArgumentException($"User not found with Id={userId}");

        var checklist = await checklistRepo.GetById(query.ChecklistId, ct)
            ?? throw new ApplicationArgumentException($"Checklist not found with Id={query.ChecklistId}");

        if (checklist.UserId != userId)
            throw new ApplicationArgumentException($"User has no Checklist with ChecklistId={query.ChecklistId}");

        return ChecklistState.CreateState(checklist);
    }
}

