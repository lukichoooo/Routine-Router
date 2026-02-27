using Application.Seedwork;
using Domain.Entities.Schedules;

namespace Application.UseCases.Schedules.Queries;


public sealed record GetTodaysChecklist : IQuery<ChecklistState>;

