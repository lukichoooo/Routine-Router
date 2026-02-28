using System.Text.Json.Serialization;
using Domain.Entities.Schedules.Enums;
using Domain.SeedWork;

namespace Domain.Entities.Schedules.ValueObjects;



public sealed class TaskType : ValueObject
{
    public string Name { get; }
    public TaskCategory Category { get; }

    [JsonConstructor]
    private TaskType(string name, TaskCategory category)
    {
        Name = name;
        Category = category;
    }

    public static TaskType DeepWork => new("Deep Work", TaskCategory.Core);
    public static TaskType Creative => new("Creative Flow", TaskCategory.Core);
    public static TaskType Learning => new("Learning", TaskCategory.Growth);

    public static TaskType ShallowWork => new("Shallow Work", TaskCategory.Admin);
    public static TaskType Meeting => new("Meeting", TaskCategory.Admin);
    public static TaskType Planning => new("Planning", TaskCategory.Admin);

    public static TaskType Sleep => new("Sleep", TaskCategory.Health);
    public static TaskType Recharge => new("Recharge", TaskCategory.Health);
    public static TaskType Exercise => new("Exercise", TaskCategory.Health);
    public static TaskType Fuel => new("Eat", TaskCategory.Health);

    public static TaskType Chores => new("Chores", TaskCategory.Life);
    public static TaskType Social => new("Social", TaskCategory.Life);
    public static TaskType Leisure => new("Leisure", TaskCategory.Life);

    public static IReadOnlyList<TaskType> All =>
    [
        DeepWork, Creative, Learning,
            ShallowWork, Meeting, Planning,
            Sleep, Recharge, Exercise, Fuel,
            Chores, Social, Leisure
    ];

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return Category;
    }

    public override string ToString() => $"{Name} ({Category})";



#pragma warning disable CS8618
    private TaskType() { }
#pragma warning restore CS8618
}

