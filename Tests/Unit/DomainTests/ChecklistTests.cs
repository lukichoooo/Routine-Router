using AutoFixture;
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
            var userId = _fix.Create<Guid>();
            var checklist = new Checklist(userId);

            Assert.That(checklist, Is.Not.Null);
            Assert.That(checklist.UserId, Is.EqualTo(userId));
            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(1));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<ChecklistCreated>());
        }

        [Test]
        public void AddTask_Success()
        {
            var userId = _fix.Create<Guid>();
            var checklist = new Checklist(userId);

            var taskName = _fix.Create<Name>();
            var taskSchedule = _fix.Create<Schedule>();
            var taskMetadata = _fix.Create<string>();
            var taskType = TaskType.DeepWork;
            checklist.AddTask(taskName, taskType, taskSchedule, taskMetadata);

            Assert.That(
                    checklist.Tasks.FirstOrDefault
                        (t => t.Name == taskName
                         && t.PlannedSchedule == taskSchedule
                         && t.Metadata == taskMetadata),
                    Is.Not.Null);

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(2));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<ChecklistCreated>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskAddedToChecklist>());

            var task = checklist.Tasks.First();
            Assert.That(task.DomainEvents, Has.Count.EqualTo(1));
            Assert.That(task.DomainEvents, Has.Exactly(1).InstanceOf<TaskCreated>());
        }


        [Test]
        public void RemoveTask_Success()
        {
            var userId = _fix.Create<Guid>();
            var checklist = new Checklist(userId);

            var taskName = _fix.Create<Name>();
            var taskSchedule = _fix.Create<Schedule>();
            var taskMetadata = _fix.Create<string>();
            var taskType = TaskType.DeepWork;
            checklist.AddTask(taskName, taskType, taskSchedule, taskMetadata);
            var task = checklist.Tasks.First();

            checklist.RemoveTask(task.Id);

            Assert.That(
                    checklist.Tasks.FirstOrDefault
                        (t => t.Name == taskName
                         && t.PlannedSchedule == taskSchedule
                         && t.Metadata == taskMetadata),
                    Is.Null);

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(3));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<ChecklistCreated>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskAddedToChecklist>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskRemovedFromChecklist>());

            Assert.That(task.DomainEvents, Has.Count.EqualTo(1));
            Assert.That(task.DomainEvents, Has.Exactly(1).InstanceOf<TaskCreated>());
        }

        [Test]
        public void StartTask_Success()
        {
            var userId = _fix.Create<Guid>();
            var checklist = new Checklist(userId);

            var taskName = _fix.Create<Name>();
            var taskSchedule = _fix.Create<Schedule>();
            var taskMetadata = _fix.Create<string>();

            var taskType = TaskType.DeepWork;
            checklist.AddTask(taskName, taskType, taskSchedule, taskMetadata);
            var task = checklist.Tasks.First();
            checklist.StartTask(task.Id);

            Assert.That(
                    checklist.Tasks.FirstOrDefault
                        (t => t.Name == taskName
                         && t.PlannedSchedule == taskSchedule
                         && t.Metadata == taskMetadata),
                    Is.Not.Null);

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(2));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<ChecklistCreated>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskAddedToChecklist>());

            Assert.That(task.DomainEvents, Has.Count.EqualTo(2));
            Assert.That(task.DomainEvents, Has.Exactly(1).InstanceOf<TaskCreated>());
            Assert.That(task.DomainEvents, Has.Exactly(1).InstanceOf<TaskStarted>());
        }


        [Test]
        public void CompleteTask_Success()
        {
            var userId = _fix.Create<Guid>();
            var checklist = new Checklist(userId);

            var taskName = _fix.Create<Name>();
            var taskSchedule = _fix.Create<Schedule>();
            var taskMetadata = _fix.Create<string>();
            var taskType = TaskType.DeepWork;
            checklist.AddTask(taskName, taskType, taskSchedule, taskMetadata);

            var task = checklist.Tasks.First();
            checklist.StartTask(task.Id);
            checklist.CompleteTask(task.Id);

            Assert.That(
                    checklist.Tasks.FirstOrDefault
                        (t => t.Name == taskName
                         && t.PlannedSchedule == taskSchedule
                         && t.Metadata == taskMetadata),
                    Is.Not.Null);

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(2));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<ChecklistCreated>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskAddedToChecklist>());

            Assert.That(task.DomainEvents, Has.Count.EqualTo(3));
            Assert.That(task.DomainEvents, Has.Exactly(1).InstanceOf<TaskCreated>());
            Assert.That(task.DomainEvents, Has.Exactly(1).InstanceOf<TaskStarted>());
            Assert.That(task.DomainEvents, Has.Exactly(1).InstanceOf<TaskCompleted>());
        }



        [Test]
        public void CompleteTask_Fail_TaskNotStarted()
        {
            var userId = _fix.Create<Guid>();
            var checklist = new Checklist(userId);

            var taskName = _fix.Create<Name>();
            var taskSchedule = _fix.Create<Schedule>();
            var taskMetadata = _fix.Create<string>();
            var taskType = TaskType.DeepWork;
            checklist.AddTask(taskName, taskType, taskSchedule, taskMetadata);

            var task = checklist.Tasks.First();

            Assert.Throws<DomainException>(
                    () => checklist.CompleteTask(task.Id));

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(2));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<ChecklistCreated>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskAddedToChecklist>());

            Assert.That(task.DomainEvents, Has.Count.EqualTo(1));
            Assert.That(task.DomainEvents, Has.Exactly(1).InstanceOf<TaskCreated>());
        }



        [Test]
        public void SetUserRating_Success()
        {
            var userId = _fix.Create<Guid>();
            var checklist = new Checklist(userId);

            var rating = new Rating(1, 2, 3, 4, 5, 6);
            checklist.SetUserRating(rating);

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(2));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<ChecklistCreated>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<UserRatingSet>());
        }


        [Test]
        public void SetLLMRating_Success()
        {
            var userId = _fix.Create<Guid>();
            var checklist = new Checklist(userId);

            var rating = new Rating(1, 2, 3, 4, 5, 6);
            checklist.SetLLMRating(rating);

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(2));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<ChecklistCreated>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<LLMRatingSet>());
        }
    }
}
