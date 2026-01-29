using AutoFixture;
using Domain.Common.ValueObjects;
using Domain.Entities.Days;
using Domain.Entities.Days.Events;
using Domain.Entities.Days.ValueObjects;

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
            Assert.That(checklist.Id, Is.EqualTo(userId));
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
            checklist.AddTask(taskName, taskSchedule, taskMetadata);

            Assert.That(checklist, Is.Not.Null);
            Assert.That(checklist.Id, Is.EqualTo(userId));
            Assert.That(
                    checklist.Tasks.FirstOrDefault
                        (t => t.Name == taskName
                         && t.PlannedSchedule == taskSchedule
                         && t.Metadata == taskMetadata),
                    Is.Not.Null);

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(2));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<ChecklistCreated>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskAddedToChecklist>());
        }


        [Test]
        public void RemoveTask_Success()
        {
            var userId = _fix.Create<Guid>();
            var checklist = new Checklist(userId);

            var taskName = _fix.Create<Name>();
            var taskSchedule = _fix.Create<Schedule>();
            var taskMetadata = _fix.Create<string>();
            checklist.AddTask(taskName, taskSchedule, taskMetadata);

            Assert.That(checklist, Is.Not.Null);
            Assert.That(checklist.Id, Is.EqualTo(userId));
            Assert.That(
                    checklist.Tasks.FirstOrDefault
                        (t => t.Name == taskName
                         && t.PlannedSchedule == taskSchedule
                         && t.Metadata == taskMetadata),
                    Is.Not.Null);

            Assert.That(checklist.DomainEvents, Has.Count.EqualTo(2));
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<ChecklistCreated>());
            Assert.That(checklist.DomainEvents, Has.Exactly(1).InstanceOf<TaskAddedToChecklist>());
        }

    }
}
