
using Domain.Common.ValueObjects;
using Domain.Entities.Checklists.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Checklists;

public class Task : Entity
{
    public Name Name { get; private set; }
    public string? Metadata { get; private set; }

    public HourOfDay StartTime { get; private set; }
    public HourOfDay EndTime { get; private set; }

    public Progress Progress { get; set; }



    private Task() { }
}

