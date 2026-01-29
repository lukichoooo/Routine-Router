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
        UserId = e.UserId;
        Statistics = new Statistics(e.TimeStamp);
    }

    public void Apply(TaskAddedToChecklist e)
    {
        _tasks.Add(new TaskEntity(
            e.TaskId,
            e.Name,
            e.TaskType,
            e.Planned,
            e.ChecklistId,
            e.Metadata
        ));
    }

    public void Apply(TaskStarted e)
        => TryGetTask(e.TaskId).StartInternal();

    public void Apply(TaskCompleted e)
        => TryGetTask(e.TaskId).CompleteInternal();

    public void Apply(TaskRemovedFromChecklist e)
        => _tasks.Remove(TryGetTask(e.TaskId));
    public void Apply(TaskUpdateMetadata e)
        => TryGetTask(e.TaskId).UpdateMetadata(e.Metadata);

    public void Apply(UserRatingSet e)
        => Statistics = Statistics.WithUserRating(e.UserRating);

    public void Apply(LLMRatingSet e)
        => Statistics = Statistics.WithLLMRating(e.LLMRating);


    // ---------- HELPERS ----------

    public TaskEntity TryGetTask(Guid id) =>
        _tasks.FirstOrDefault(t => t.Id == id)
        ?? throw new DomainArgumentException($"Task {id} not found");
}

