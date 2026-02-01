using AutoFixture;
using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Common.ValueObjects;
using Domain.Entities.Schedules;
using Domain.Entities.Schedules.Events;
using Domain.Entities.Schedules.ValueObjects;
using Domain.SeedWork;
using FixtureProvider;

namespace DomainTests
{
    [TestFixture]
    public class ChecklistTests
    {
        private readonly Fixture _fix = FixtureFactory.GetFixture();

        [Test]
        public void Create_Success()
        {
            var evt = _fix.Create<ChecklistCreated>();
            var checklist = new Checklist([evt]);

            Assert.That(checklist, Is.Not.Null);
            Assert.That(checklist.DomainEvents, Is.Empty);
            Assert.That(checklist.Version, Is.EqualTo(evt.Version));
        }

        [Test]
        public void AddTask_Success()
        {
            var evt = _fix.Create<ChecklistCreated>();
            var checklist = new Checklist([evt]);

            var taskName = _fix.Create<Name>();
            var taskSchedule = _fix.Create<Schedule>();
            var taskMetadata = _fix.Create<string>();
            var taskType = TaskType.DeepWork;
            checklist.AddTask(taskName, taskType, taskSchedule, taskMetadata);

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(1));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskAddedToChecklist>());
            Assert.That(checklist.Version, Is.EqualTo(evt.Version + checklist.DomainEvents.Count));
        }

        [Test]
        public void AddTask_Success_FromHistory()
        {
            var evt = _fix.Create<ChecklistCreated>();
            var evt2 = _fix.Create<TaskAddedToChecklist>();
            IEnumerable<IDomainEvent<ChecklistId>> events = [evt, evt2];
            var checklist = new Checklist(events);

            Assert.That(checklist.DomainEvents, Is.Empty);
            Assert.That(checklist.Version, Is.EqualTo(events.Last().Version + checklist.DomainEvents.Count));
        }



        [Test]
        public void RemoveTask_Success()
        {
            var evt = _fix.Create<ChecklistCreated>();
            IEnumerable<IDomainEvent<ChecklistId>> events = [evt];
            var checklist = new Checklist(events);

            var taskName = _fix.Create<Name>();
            var taskSchedule = _fix.Create<Schedule>();
            var taskMetadata = _fix.Create<string>();
            var taskType = TaskType.DeepWork;
            TaskId taskId = checklist.AddTask(taskName, taskType, taskSchedule, taskMetadata);

            checklist.RemoveTask(taskId);

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(2));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskAddedToChecklist>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskRemovedFromChecklist>());
            Assert.That(checklist.Version, Is.EqualTo(events.Last().Version + checklist.DomainEvents.Count));
        }

        [Test]
        public void StartTask_Success()
        {
            var evt = _fix.Create<ChecklistCreated>();
            IEnumerable<IDomainEvent<ChecklistId>> events = [evt];
            var checklist = new Checklist(events);

            var taskName = _fix.Create<Name>();
            var taskSchedule = _fix.Create<Schedule>();
            var taskMetadata = _fix.Create<string>();
            var taskType = TaskType.DeepWork;

            var taskId = checklist.AddTask(taskName, taskType, taskSchedule, taskMetadata);
            checklist.StartTask(taskId);

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(2));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskAddedToChecklist>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskStarted>());
            Assert.That(checklist.Version, Is.EqualTo(events.Last().Version + checklist.DomainEvents.Count));
        }


        [Test]
        public void CompleteTask_Success()
        {
            var evt = _fix.Create<ChecklistCreated>();
            IEnumerable<IDomainEvent<ChecklistId>> events = [evt];
            var checklist = new Checklist(events);

            var taskName = _fix.Create<Name>();
            var taskSchedule = _fix.Create<Schedule>();
            var taskMetadata = _fix.Create<string>();
            var taskType = TaskType.DeepWork;
            var taskId = checklist.AddTask(taskName, taskType, taskSchedule, taskMetadata);

            checklist.StartTask(taskId);
            checklist.CompleteTask(taskId);

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(3));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskAddedToChecklist>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskStarted>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskCompleted>());
            Assert.That(checklist.Version, Is.EqualTo(events.Last().Version + checklist.DomainEvents.Count));
        }



        [Test]
        public void CompleteTask_Fail_TaskNotStarted()
        {
            var evt = _fix.Create<ChecklistCreated>();
            IEnumerable<IDomainEvent<ChecklistId>> events = [evt];
            var checklist = new Checklist(events);

            var taskName = _fix.Create<Name>();
            var taskSchedule = _fix.Create<Schedule>();
            var taskMetadata = _fix.Create<string>();
            var taskType = TaskType.DeepWork;
            var taskId = checklist.AddTask(taskName, taskType, taskSchedule, taskMetadata);

            Assert.Throws<DomainRuleViolation>(() => checklist.CompleteTask(taskId));

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(1));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskAddedToChecklist>());
            Assert.That(checklist.Version, Is.EqualTo(events.Last().Version + checklist.DomainEvents.Count));
        }



        [Test]
        public void SetUserRating_Success()
        {
            var evt = _fix.Create<ChecklistCreated>();
            IEnumerable<IDomainEvent<ChecklistId>> events = [evt];
            var checklist = new Checklist(events);

            var rating = _fix.Create<Rating>();
            checklist.SetUserRating(rating);

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(1));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<UserRatingSet>());
            Assert.That(checklist.Version, Is.EqualTo(events.Last().Version + checklist.DomainEvents.Count));
        }


        [Test]
        public void SetLLMRating_Success()
        {
            var evt = _fix.Create<ChecklistCreated>();
            IEnumerable<IDomainEvent<ChecklistId>> events = [evt];
            var checklist = new Checklist(events);

            var rating = _fix.Create<Rating>();
            checklist.SetLLMRating(rating);

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(1));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<LLMRatingSet>());
            Assert.That(checklist.Version, Is.EqualTo(events.Last().Version + checklist.DomainEvents.Count));
        }


        // ----------- STATE TESTS



        [Test]
        public void State_Create_Success()
        {
            var evt = _fix.Create<ChecklistCreated>();
            var state = new ChecklistState();
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
            var state = new ChecklistState();
            state.Apply(evt);

            var addTaskToChecklist = _fix.Create<TaskAddedToChecklist>();
            state.Apply(addTaskToChecklist);

            Assert.That(state, Is.Not.Null);
            Assert.That(state.Tasks, Has.Count.EqualTo(1));
            Assert.That(state.Tasks.First().IsCompleted, Is.False);
            Assert.That(state.Tasks.First().Id, Is.EqualTo(addTaskToChecklist.TaskId));
        }


        [Test]
        public void State_RemoveTask_Success()
        {
            var evt = _fix.Create<ChecklistCreated>();
            var state = new ChecklistState();
            state.Apply(evt);

            var addTaskToChecklist = _fix.Create<TaskAddedToChecklist>();
            state.Apply(addTaskToChecklist);

            var removeTaskFromChecklist = new TaskRemovedFromChecklist(
                        addTaskToChecklist.AggregateId,
                        addTaskToChecklist.Version + 1,
                        Clock.Now,
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
            var state = new ChecklistState();
            state.Apply(evt);

            var addTaskToChecklist = _fix.Create<TaskAddedToChecklist>();
            state.Apply(addTaskToChecklist);

            var startTask = new TaskStarted(
                            evt.AggregateId,
                            addTaskToChecklist.Version + 1,
                            Clock.Now,
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
            var state = new ChecklistState();
            state.Apply(evt);

            var addTaskToChecklist = _fix.Create<TaskAddedToChecklist>();
            state.Apply(addTaskToChecklist);

            var startTask = new TaskStarted(
                            evt.AggregateId,
                            addTaskToChecklist.Version + 1,
                            Clock.Now,
                            addTaskToChecklist.TaskId
                            );
            state.Apply(startTask);

            var completeTask = new TaskCompleted(
                            evt.AggregateId,
                            startTask.Version + 1,
                            Clock.Now,
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
            var state = new ChecklistState();
            state.Apply(evt);


            var rating = new UserRatingSet(
                            evt.AggregateId,
                            100,
                            Clock.Now,
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
            var state = new ChecklistState();
            state.Apply(evt);


            var rating = new LLMRatingSet(
                            evt.AggregateId,
                            100,
                            Clock.Now,
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
            var state = new ChecklistState();
            state.Apply(evt);

            var addTaskToChecklist = _fix.Create<TaskAddedToChecklist>();
            state.Apply(addTaskToChecklist);

            var metadata = _fix.Create<string>();
            var updateMetadata = new TaskMetadataUpdated(
                            evt.AggregateId,
                            100,
                            Clock.Now,
                            addTaskToChecklist.TaskId,
                            metadata);
            state.Apply(updateMetadata);

            Assert.That(state, Is.Not.Null);
            Assert.That(state.Tasks.First().Metadata, Is.Not.Null);
            Assert.That(state.Tasks.First().Metadata, Is.EqualTo(metadata));
        }
    }
}
