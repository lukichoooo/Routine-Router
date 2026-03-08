using Application.Common.Exceptions;
using Application.Seedwork;
using Application.UseCases.Identity;
using Domain.Common;

namespace Application.UseCases.Schedules.Queries;


public sealed record GetTodaysChecklists : IQuery<IEnumerable<ChecklistDto>>;


public class GetTodaysChecklistsQueryHandler(
        IIdentityProvider identity,
        IChecklistRepo checklistRepo,
        IUserRepo userRepo)
: BaseQueryHandler<GetTodaysChecklists, IEnumerable<ChecklistDto>>
{
    protected override async Task<IEnumerable<ChecklistDto>> Execute(
            GetTodaysChecklists query,
            CancellationToken ct)
    {
        var userId = identity.GetCurrentUserId();
        if (await userRepo.GetById(userId, ct) is null)
            throw new ApplicationArgumentException($"User not found with Id={userId}");

        var currentDate = Clock.CurrentDate;
        var checklists = await checklistRepo.GetForDay(userId, currentDate, ct)
            ?? throw new ApplicationArgumentException($"Checklist not found with UserId={userId} and Date={currentDate}");

        return checklists.Select(c => ChecklistDto.From(c.State));
    }
}

