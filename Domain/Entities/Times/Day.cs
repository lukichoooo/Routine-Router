using Domain.Common.ValueObjects;
using Domain.Entities.Checklists;
using Domain.Entities.Checklists.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Times;

public class Day : Entity, IAggregateRoot
{
    public DateOnly Date { get; private set; }

    public HourOfDay SleepTime { get; private set; }
    public HourOfDay WakeTime { get; private set; }

    public Rating? UserRating { get; private set; }
    public Rating LLMRating { get; private set; }

    public Checklist Checklist { get; private set; }



    private Day() { }
}

