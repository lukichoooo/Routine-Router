using Domain.Common.Exceptions;
using Domain.Entities.Schedules.Events;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Schedules;

public sealed class ChecklistState : AggregateRootState<ChecklistId>, IAggregateRootStateFactory<ChecklistState, ChecklistId>
{
    private readonly List<TaskEntity> _tasks = [];
    public IReadOnlyList<TaskEntity> Tasks => _tasks;

    public UserId UserId { get; private set; }
    public Statistics Statistics { get; private set; }


    public static ChecklistState CreateState(AggregateRoot<ChecklistId> owner)
        => new(owner);


    // ---------- APPLY  ----------

    public void Apply(ChecklistCreated e)
    {
        Id = e.AggregateId;
        UserId = e.UserId;
        Statistics = new Statistics(e.Timestamp);
    }

    public void Apply(TaskAddedToChecklist e)
    {
        _tasks.Add(new TaskEntity(
            id: e.TaskId,
            name: e.Name,
            taskType: e.TaskType,
            planned: e.Planned,
            checklistId: e.AggregateId,
            metadata: e.Metadata
        ));
    }

    public void Apply(TaskStarted e)
        => TryGetTask(e.TaskId).Start(e.Timestamp);

    public void Apply(TaskCompleted e)
        => TryGetTask(e.TaskId).Complete(e.Timestamp);

    public void Apply(TaskRemovedFromChecklist e)
        => _tasks.Remove(TryGetTask(e.TaskId));

    public void Apply(TaskMetadataUpdated e)
        => TryGetTask(e.TaskId).UpdateMetadataInternal(e.Metadata);

    public void Apply(UserRatingSet e)
        => Statistics = Statistics.WithUserRating(e.UserRating);

    public void Apply(LLMRatingSet e)
        => Statistics = Statistics.WithLLMRating(e.LLMRating);


    // ---------- HELPERS ----------

    public TaskEntity TryGetTask(TaskId id) =>
        _tasks.FirstOrDefault(t => t.Id == id)
        ?? throw new DomainRuleViolation($"Task {id} not found");

#pragma warning disable CS8618 
    private ChecklistState(AggregateRoot<ChecklistId> owner) : base(owner) { }
    private ChecklistState() { }
#pragma warning restore CS8618
}

