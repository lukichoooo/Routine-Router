using AutoFixture;
using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Common.ValueObjects;
using Domain.Entities.Schedules;
using Domain.Entities.Schedules.Events;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;
using TestHelperFactory;

namespace DomainTests;

[TestFixture]
public class ChecklistTests
{
    private readonly Fixture _fix = TestFactory.GetFixture();

    [Test]
    public void Create_Success()
    {
        var checklist = new Checklist();
        checklist.Create(_fix.Create<ChecklistId>(), _fix.Create<UserId>());

        Assert.That(checklist, Is.Not.Null);
        Assert.That(checklist.DomainEvents, Has.Count.EqualTo(1));
        Assert.That(checklist.Version, Is.EqualTo(1));
    }

    [TestCase(1)]
    [TestCase(3)]
    public void AddTask_Success(int tasksCount)
    {
        var checklist = new Checklist();
        checklist.Create(_fix.Create<ChecklistId>(), _fix.Create<UserId>());

        for (int i = 0; i < tasksCount; i++)
        {
            var taskName = _fix.Create<Name>();
            var taskSchedule = _fix.Create<Schedule>();
            var taskMetadata = _fix.Create<string>();
            var taskType = TaskType.DeepWork;
            checklist.AddTask(taskName, taskType, taskSchedule, taskMetadata);
        }

        Assert.That(checklist.DomainEvents, Has.Count.EqualTo(1 + tasksCount));
        Assert.That(checklist.DomainEvents,
                Has.Exactly(1).InstanceOf<ChecklistCreated>());
        Assert.That(checklist.DomainEvents,
                Has.Exactly(tasksCount).InstanceOf<TaskAddedToChecklist>());
        Assert.That(checklist.Version, Is.EqualTo(checklist.DomainEvents.Count));
    }

    [TestCase(1)]
    [TestCase(3)]
    public void AddTask_Success_FromHistory(int tasksCount)
    {
        var checklist = new Checklist();
        checklist.Create(_fix.Create<ChecklistId>(), _fix.Create<UserId>());
        for (int i = 0; i < tasksCount; i++)
        {
            var taskName = _fix.Create<Name>();
            var taskSchedule = _fix.Create<Schedule>();
            var taskMetadata = _fix.Create<string>();
            var taskType = TaskType.DeepWork;
            checklist.AddTask(taskName, taskType, taskSchedule, taskMetadata);
        }
        Assert.That(checklist.DomainEvents, Has.Count.EqualTo(1 + tasksCount));

        IEnumerable<IDomainEvent> events = checklist.DomainEvents;
        var checklistFromHistory = new Checklist(events);

        Assert.That(checklistFromHistory.DomainEvents, Is.Empty);
        Assert.That(checklistFromHistory.Version, Is.EqualTo(1 + tasksCount));
        Assert.That(checklistFromHistory.Version, Is.EqualTo(checklist.Version));
        Assert.That(checklistFromHistory.Version, Is.EqualTo(checklistFromHistory.StoredVersion));
    }



    [TestCase(1, 1)]
    [TestCase(3, 1)]
    [TestCase(1, 3)]
    public void RemoveTask_Ranges(int addCount, int removeCount)
    {
        var checklist = new Checklist();
        checklist.Create(_fix.Create<ChecklistId>(), _fix.Create<UserId>());

        List<TaskId> addedTaskIds = [];
        for (int i = 0; i < addCount; i++)
        {
            var taskName = _fix.Create<Name>();
            var taskSchedule = _fix.Create<Schedule>();
            var taskMetadata = _fix.Create<string>();
            var taskType = TaskType.DeepWork;
            TaskId taskId = checklist.AddTask(taskName, taskType, taskSchedule, taskMetadata);
            addedTaskIds.Add(taskId);
        }

        if (removeCount > addCount)
        {
            try
            {
                for (int i = 0; i < removeCount; i++)
                {
                    if (i < addedTaskIds.Count)
                        checklist.RemoveTask(addedTaskIds[i]);
                    else
                        checklist.RemoveTask(_fix.Create<TaskId>());
                }
            }
            catch (Exception ex)
            {
                Assert.That(ex, Is.TypeOf<DomainRuleViolation>());
            }
        }
        else
        {
            for (int i = 0; i < removeCount; i++)
                checklist.RemoveTask(addedTaskIds[i]);

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(addCount + removeCount + 1));
            Assert.That(checklist.DomainEvents,
                    Has.Exactly(addCount).InstanceOf<TaskAddedToChecklist>());
            Assert.That(checklist.DomainEvents,
                    Has.Exactly(removeCount).InstanceOf<TaskRemovedFromChecklist>());
            Assert.That(checklist.Version, Is.EqualTo(checklist.DomainEvents.Count));
        }

    }

    [Test]
    public void StartTask_Success()
    {
        var checklist = new Checklist();
        checklist.Create(_fix.Create<ChecklistId>(), _fix.Create<UserId>());

        var taskName = _fix.Create<Name>();
        var taskSchedule = _fix.Create<Schedule>();
        var taskMetadata = _fix.Create<string>();
        var taskType = TaskType.DeepWork;

        var taskId = checklist.AddTask(taskName, taskType, taskSchedule, taskMetadata);
        checklist.StartTask(taskId);

        Assert.That(checklist.DomainEvents, Has.Count.EqualTo(3));
        Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskAddedToChecklist>());
        Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskStarted>());
        Assert.That(checklist.Version, Is.EqualTo(checklist.DomainEvents.Count));
    }


    [Test]
    public void CompleteTask_Success()
    {
        var checklist = new Checklist();
        checklist.Create(_fix.Create<ChecklistId>(), _fix.Create<UserId>());

        var taskName = _fix.Create<Name>();
        var taskSchedule = _fix.Create<Schedule>();
        var taskMetadata = _fix.Create<string>();
        var taskType = TaskType.DeepWork;

        var taskId = checklist.AddTask(taskName, taskType, taskSchedule, taskMetadata);
        checklist.StartTask(taskId);
        checklist.CompleteTask(taskId);

        Assert.That(checklist.DomainEvents, Has.Count.EqualTo(4));
        Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskAddedToChecklist>());
        Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskStarted>());
        Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskCompleted>());
        Assert.That(checklist.Version, Is.EqualTo(checklist.DomainEvents.Count));
    }



    [Test]
    public void CompleteTask_Fail_TaskNotStarted()
    {
        var checklist = new Checklist();
        checklist.Create(_fix.Create<ChecklistId>(), _fix.Create<UserId>());

        var taskName = _fix.Create<Name>();
        var taskSchedule = _fix.Create<Schedule>();
        var taskMetadata = _fix.Create<string>();
        var taskType = TaskType.DeepWork;

        var taskId = checklist.AddTask(taskName, taskType, taskSchedule, taskMetadata);

        Assert.Throws<DomainRuleViolation>(() => checklist.CompleteTask(taskId));

        Assert.That(checklist.DomainEvents, Has.Count.EqualTo(2));
        Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskAddedToChecklist>());
        Assert.That(checklist.Version, Is.EqualTo(checklist.DomainEvents.Count));
    }



    [Test]
    public void SetUserRating_Success()
    {
        var checklist = new Checklist();
        checklist.Create(_fix.Create<ChecklistId>(), _fix.Create<UserId>());
        checklist.SetUserRating(_fix.Create<Rating>());

        Assert.That(checklist.DomainEvents, Has.Count.EqualTo(2));
        Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<UserRatingSet>());
        Assert.That(checklist.Version, Is.EqualTo(checklist.DomainEvents.Count));
    }


    [Test]
    public void SetLLMRating_Success()
    {
        var checklist = new Checklist();
        checklist.Create(_fix.Create<ChecklistId>(), _fix.Create<UserId>());
        checklist.SetLLMRating(_fix.Create<Rating>());

        Assert.That(checklist.DomainEvents, Has.Count.EqualTo(2));
        Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<LLMRatingSet>());
        Assert.That(checklist.Version, Is.EqualTo(checklist.DomainEvents.Count));
    }


    // ----------- STATE TESTS



    [Test]
    public void State_Create_Success()
    {
        var evt = _fix.Create<ChecklistCreated>();
        var state = ChecklistState.CreateState(null!);
        state.Apply(evt);

        Assert.That(state, Is.Not.Null);
        Assert.That(state.Id, Is.EqualTo(evt.AggregateId));
        Assert.That(state.UserId, Is.EqualTo(evt.UserId));
        Assert.That(state.Statistics.CreatedAt, Is.EqualTo(evt.Timestamp));
    }

    [Test]
    public void State_AddTask_Success()
    {
        var evt = _fix.Create<ChecklistCreated>();
        var state = ChecklistState.CreateState(null!);
        state.Apply(evt);

        var addTaskToChecklist = _fix.Create<TaskAddedToChecklist>();
        state.Apply(addTaskToChecklist);

        Assert.That(state, Is.Not.Null);
        Assert.That(state.Tasks, Has.Count.EqualTo(1));
        Assert.That(state.Tasks[0].IsCompleted, Is.False);
        Assert.That(state.Tasks[0].Id, Is.EqualTo(addTaskToChecklist.TaskId));
    }


    [Test]
    public void State_RemoveTask_Success()
    {
        var evt = _fix.Create<ChecklistCreated>();
        var state = ChecklistState.CreateState(null!);
        state.Apply(evt);

        var addTaskToChecklist = _fix.Create<TaskAddedToChecklist>();
        state.Apply(addTaskToChecklist);

        var removeTaskFromChecklist = new TaskRemovedFromChecklist(
                    addTaskToChecklist.AggregateId,
                    addTaskToChecklist.Version + 1,
                    Clock.CurrentTime,
                    addTaskToChecklist.TaskId
                    );
        state.Apply(removeTaskFromChecklist);

        Assert.That(state, Is.Not.Null);
        Assert.That(state.Tasks, Is.Empty);
    }

    [Test]
    public void State_StartTask_Success()
    {
        var evt = _fix.Create<ChecklistCreated>();
        var state = ChecklistState.CreateState(null!);
        state.Apply(evt);

        var addTaskToChecklist = _fix.Create<TaskAddedToChecklist>();
        state.Apply(addTaskToChecklist);

        var startTask = new TaskStarted(
                        evt.AggregateId,
                        addTaskToChecklist.Version + 1,
                        Clock.CurrentTime,
                        addTaskToChecklist.TaskId
                        );
        state.Apply(startTask);

        Assert.That(state, Is.Not.Null);
        Assert.That(state.Tasks, Has.Count.EqualTo(1));
        Assert.That(state.Tasks.First().ActualSchedule, Is.Not.Null);
        Assert.That(state.Tasks.First().ActualSchedule!.StartTime, Is.EqualTo(startTask.Timestamp));
        Assert.That(state.Tasks.First().IsCompleted, Is.False);
    }


    [Test]
    public void State_CompleteTask_Success()
    {
        var evt = _fix.Create<ChecklistCreated>();
        var state = ChecklistState.CreateState(null!);
        state.Apply(evt);

        var addTaskToChecklist = _fix.Create<TaskAddedToChecklist>();
        state.Apply(addTaskToChecklist);

        var startTask = new TaskStarted(
                        evt.AggregateId,
                        addTaskToChecklist.Version + 1,
                        Clock.CurrentTime,
                        addTaskToChecklist.TaskId
                        );
        state.Apply(startTask);

        var completeTask = new TaskCompleted(
                        evt.AggregateId,
                        startTask.Version + 1,
                        Clock.CurrentTime,
                        startTask.TaskId
                        );
        state.Apply(completeTask);

        Assert.That(state, Is.Not.Null);
        Assert.That(state.Tasks, Has.Count.EqualTo(1));
        Assert.That(state.Tasks.First().ActualSchedule, Is.Not.Null);
        Assert.That(state.Tasks.First().ActualSchedule!.StartTime, Is.EqualTo(startTask.Timestamp));
        Assert.That(state.Tasks.First().ActualSchedule!.EndTime, Is.EqualTo(completeTask.Timestamp));
        Assert.That(state.Tasks.First().IsCompleted, Is.True);
    }


    [Test]
    public void State_SetUserRating_Success()
    {
        var evt = _fix.Create<ChecklistCreated>();
        var state = ChecklistState.CreateState(null!);
        state.Apply(evt);


        var rating = new UserRatingSet(
                        evt.AggregateId,
                        100,
                        Clock.CurrentTime,
                        new(1, 2, 3, 4, 5, 6));
        state.Apply(rating);

        Assert.That(state, Is.Not.Null);
        Assert.That(state.Statistics.UserRating, Is.EqualTo(rating.UserRating));
        Assert.That(state.Statistics.LLMRating, Is.Null);
    }


    [Test]
    public void State_SetLLMRating_Success()
    {
        var evt = _fix.Create<ChecklistCreated>();
        var state = ChecklistState.CreateState(null!);
        state.Apply(evt);


        var rating = new LLMRatingSet(
                        evt.AggregateId,
                        100,
                        Clock.CurrentTime,
                        new(1, 2, 3, 4, 5, 6));
        state.Apply(rating);

        Assert.That(state, Is.Not.Null);
        Assert.That(state.Statistics.LLMRating, Is.EqualTo(rating.LLMRating));
        Assert.That(state.Statistics.UserRating, Is.Null);
    }


    [Test]
    public void State_SetMetadata()
    {
        var evt = _fix.Create<ChecklistCreated>();
        var state = ChecklistState.CreateState(null!);
        state.Apply(evt);

        var addTaskToChecklist = _fix.Create<TaskAddedToChecklist>();
        state.Apply(addTaskToChecklist);

        var metadata = _fix.Create<string>();
        var updateMetadata = new TaskMetadataUpdated(
                        evt.AggregateId,
                        100,
                        Clock.CurrentTime,
                        addTaskToChecklist.TaskId,
                        metadata);
        state.Apply(updateMetadata);

        Assert.That(state, Is.Not.Null);
        Assert.That(state.Tasks.First().Metadata, Is.Not.Null);
        Assert.That(state.Tasks.First().Metadata, Is.EqualTo(metadata));
    }
}

