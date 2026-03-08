using Attributes.GeneratorAttributes;
using Domain.Entities.Schedules;

namespace Application.UseCases.Schedules.Queries;

[GenerateDto(typeof(ChecklistState),
        Exclude = [
            nameof(ChecklistState.Owner),
            nameof(ChecklistState.Version),
        ])]
public partial class ChecklistDto;

