using Domain.Common.Exceptions;
using Domain.Entities.Days.Events;
using Domain.Entities.Days.ValueObjects;

namespace Domain.Entities.Days;

public class ChecklistState
{
    public Guid Id { get; private set; }

    private readonly List<TaskEntity> _tasks = [];
    public IReadOnlyCollection<TaskEntity> Tasks => _tasks;

    public Guid UserId { get; private set; }
    public Statistics Statistics { get; private set; } = null!;

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
    {
        TryGetTask(e.TaskId).StartInternal(e.Timestamp);
    }

    public void Apply(TaskCompleted e)
    {
        TryGetTask(e.TaskId).CompleteInternal(e.Timestamp);
    }

    public void Apply(TaskRemovedFromChecklist e)
    {
        _tasks.Remove(TryGetTask(e.TaskId));
    }

    public void Apply(TaskUpdateMetadata e)
    {
        TryGetTask(e.TaskId).UpdateMetadata(e.Metadata);
    }

    public void Apply(UserRatingSet e)
    {
        Statistics = Statistics.WithUserRating(e.UserRating);
    }

    public void Apply(LLMRatingSet e)
    {
        Statistics = Statistics.WithLLMRating(e.LLMRating);
    }


    // ---------- HELPERS ----------

    public TaskEntity TryGetTask(Guid id) =>
        _tasks.FirstOrDefault(t => t.Id == id)
        ?? throw new DomainArgumentException($"Task {id} not found");

#pragma warning disable CS8618
    public ChecklistState() { }
#pragma warning restore CS8618
}

