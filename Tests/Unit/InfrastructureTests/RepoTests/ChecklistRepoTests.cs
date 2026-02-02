using Application.Interfaces.Events;
using AutoFixture;
using Domain.Common.ValueObjects;
using Domain.Entities.Schedules;
using Domain.Entities.Schedules.Events;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users.ValueObjects;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Repos;
using TestHelperFactory;

namespace InfrastructureTests.RepoTests;


[TestFixture]
public class ChecklistRepoTests // TODO:
{
    private readonly Fixture _fix = TestFactory.GetFixture();
    private readonly IEventStore _store = TestFactory.GetEventStore();
    private readonly ITrackedEntities _trackedEntities = TestFactory.GetTrackedEntities();
    private readonly EventsContext _context = TestFactory.GetEventsContext();

    [OneTimeTearDown]
    public async Task OneTimeTearDownAsync()
    {
        await _context.DisposeAsync();
    }

    [Test]
    public async Task SaveAsync_Empty()
    {
        // Arrange
        var checklist = new Checklist();
        var checklistId = _fix.Create<ChecklistId>();
        var userId = _fix.Create<UserId>();
        checklist.Create(checklistId, userId);

        var sut = new ChecklistRepo(_store, _trackedEntities);

        // Act
        await sut.SaveAsync(checklist, default);


        // Assert
        var res = await _store.LoadAsync(checklist.Id, default);

        var trackedEntities = _trackedEntities.GetDictionary();
        var checklistEntities = trackedEntities[typeof(Checklist)];

        Assert.That(checklistEntities, Does.Contain(checklist));
        Assert.That(res, Is.Empty);
    }

    [Test]
    public async Task SaveAsync_Events()
    {
        // Arrange
        var checklist = new Checklist();
        var checklistId = _fix.Create<ChecklistId>();
        var userId = _fix.Create<UserId>();
        checklist.Create(checklistId, userId);

        var sut = new ChecklistRepo(_store, _trackedEntities);

        // Act
        await sut.SaveAsync(checklist, default);


        // Assert
        var res = await _store.LoadAsync(checklist.Id, default);

        var trackedEntities = _trackedEntities.GetDictionary();
        var checklistEntities = trackedEntities.GetValueOrDefault(typeof(Checklist));

        Assert.That(checklistEntities, Does.Contain(checklist));
        Assert.That(res, Is.Empty);
    }


    [TestCase(1)]
    [TestCase(3)]
    public async Task GetByIdAsync_Events(int eventsCount)
    {
        // Arrange
        var checklistId = _fix.Create<ChecklistId>();

        var oldChecklist = new Checklist();
        oldChecklist.Create(checklistId, _fix.Create<UserId>());

        for (int i = 1; i < eventsCount; ++i)
        {
            oldChecklist.AddTask(
                    _fix.Create<Name>(),
                    _fix.Create<TaskType>(),
                    _fix.Create<Schedule>(),
                    _fix.Create<string>()
                    );
        }

        var sut = new ChecklistRepo(_store, _trackedEntities);
        await sut.SaveAsync(oldChecklist, default);
        await _context.SaveChangesAsync();


        // Act
        var checklist = await sut.GetByIdAsync(checklistId, default);

        // Assert

        Assert.That(checklist, Is.Not.Null);
        Assert.That(checklist.DomainEvents, Has.Exactly(eventsCount).Items);
    }




}

