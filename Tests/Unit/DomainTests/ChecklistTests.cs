using AutoFixture;
using Domain.Common.Exceptions;
using Domain.Common.ValueObjects;
using Domain.Entities.Days;
using Domain.Entities.Days.Events;
using Domain.Entities.Days.ValueObjects;
using Domain.SeedWork;

namespace DomainTests
{
    [TestFixture]
    public class ChecklistTests
    {
        private readonly Fixture _fix = new();

        [OneTimeSetUp]
        public void Setup()
        {
            _fix.Behaviors.Add(new OmitOnRecursionBehavior());
        }


        [Test]
        public void Create_Success()
        {
            var cmd = _fix.Create<ChecklistCreated>();
            IEnumerable<IDomainEvent> commands = [cmd];
            var checklist = new Checklist(commands);

            Assert.That(checklist, Is.Not.Null);
            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(1));
            Assert.That(checklist.DomainEvents, Is.EquivalentTo(commands));
        }

        [Test]
        public void AddTask_Success()
        {
            var cmd = _fix.Create<ChecklistCreated>();
            IEnumerable<IDomainEvent> commands = [cmd];
            var checklist = new Checklist(commands);

            var taskName = _fix.Create<Name>();
            var taskSchedule = _fix.Create<Schedule>();
            var taskMetadata = _fix.Create<string>();
            var taskType = TaskType.DeepWork;
            checklist.AddTask(taskName, taskType, taskSchedule, taskMetadata);

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(2));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<ChecklistCreated>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskAddedToChecklist>());
        }

        [Test]
        public void AddTask_Success_FromHistory()
        {
            var cmd = _fix.Create<ChecklistCreated>();
            var cmd2 = _fix.Create<TaskAddedToChecklist>();
            IEnumerable<IDomainEvent> commands = [cmd, cmd2];
            var checklist = new Checklist(commands);

            Assert.That(checklist.DomainEvents, Is.EquivalentTo(commands));
        }



        [Test]
        public void RemoveTask_Success()
        {
            var cmd = _fix.Create<ChecklistCreated>();
            IEnumerable<IDomainEvent> commands = [cmd];
            var checklist = new Checklist(commands);

            var taskName = _fix.Create<Name>();
            var taskSchedule = _fix.Create<Schedule>();
            var taskMetadata = _fix.Create<string>();
            var taskType = TaskType.DeepWork;
            Guid taskId = checklist.AddTask(taskName, taskType, taskSchedule, taskMetadata);

            checklist.RemoveTask(taskId);

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(3));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<ChecklistCreated>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskAddedToChecklist>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskRemovedFromChecklist>());
        }

        [Test]
        public void StartTask_Success()
        {
            var cmd = _fix.Create<ChecklistCreated>();
            IEnumerable<IDomainEvent> commands = [cmd];
            var checklist = new Checklist(commands);

            var taskName = _fix.Create<Name>();
            var taskSchedule = _fix.Create<Schedule>();
            var taskMetadata = _fix.Create<string>();
            var taskType = TaskType.DeepWork;

            var taskId = checklist.AddTask(taskName, taskType, taskSchedule, taskMetadata);
            checklist.StartTask(taskId);

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(3));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<ChecklistCreated>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskAddedToChecklist>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskStarted>());
        }


        [Test]
        public void CompleteTask_Success()
        {
            var cmd = _fix.Create<ChecklistCreated>();
            IEnumerable<IDomainEvent> commands = [cmd];
            var checklist = new Checklist(commands);

            var taskName = _fix.Create<Name>();
            var taskSchedule = _fix.Create<Schedule>();
            var taskMetadata = _fix.Create<string>();
            var taskType = TaskType.DeepWork;
            var taskId = checklist.AddTask(taskName, taskType, taskSchedule, taskMetadata);

            checklist.StartTask(taskId);
            checklist.CompleteTask(taskId);

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(4));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<ChecklistCreated>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskAddedToChecklist>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskStarted>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskCompleted>());
        }



        [Test]
        public void CompleteTask_Fail_TaskNotStarted()
        {
            var cmd = _fix.Create<ChecklistCreated>();
            IEnumerable<IDomainEvent> commands = [cmd];
            var checklist = new Checklist(commands);

            var taskName = _fix.Create<Name>();
            var taskSchedule = _fix.Create<Schedule>();
            var taskMetadata = _fix.Create<string>();
            var taskType = TaskType.DeepWork;
            var taskId = checklist.AddTask(taskName, taskType, taskSchedule, taskMetadata);

            Assert.Throws<DomainRuleViolation>(() => checklist.CompleteTask(taskId));

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(2));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<ChecklistCreated>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskAddedToChecklist>());
        }



        [Test]
        public void SetUserRating_Success()
        {
            var cmd = _fix.Create<ChecklistCreated>();
            IEnumerable<IDomainEvent> commands = [cmd];
            var checklist = new Checklist(commands);

            var rating = new Rating(1, 2, 3, 4, 5, 6);
            checklist.SetUserRating(rating);

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(2));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<ChecklistCreated>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<UserRatingSet>());
        }


        [Test]
        public void SetLLMRating_Success()
        {
            var cmd = _fix.Create<ChecklistCreated>();
            IEnumerable<IDomainEvent> commands = [cmd];
            var checklist = new Checklist(commands);

            var rating = new Rating(1, 2, 3, 4, 5, 6);
            checklist.SetLLMRating(rating);

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(2));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<ChecklistCreated>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<LLMRatingSet>());
        }
    }
}
