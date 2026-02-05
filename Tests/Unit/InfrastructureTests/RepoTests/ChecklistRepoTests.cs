using Application.Interfaces.Events;
using AutoFixture;
using Domain.Common.ValueObjects;
using Domain.Entities.Schedules;
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

    private readonly IEventStore _eventStore = TestFactory.GetEventStore();
    private readonly IEntityStateStore<ChecklistState, ChecklistId> _stateStore
        = TestFactory.GetChecklistStateStore();

    private readonly EventsContext _eventsContext = TestFactory.GetEventsContext();
    private readonly EntitiesContext _entitiesContext = TestFactory.GetEntitiesContext();

    [OneTimeTearDown]
    public async Task OneTimeTearDownAsync()
    {
        await _eventsContext.DisposeAsync();
        await _entitiesContext.DisposeAsync();
    }

    [Test]
    public async Task SaveAsync_Empty()
    {
        // Arrange
        var checklist = new Checklist();
        var checklistId = _fix.Create<ChecklistId>();
        var userId = _fix.Create<UserId>();
        checklist.Create(checklistId, userId);

        var sut = new ChecklistRepo(_eventStore, _stateStore);

        // Act
        await sut.AddAsync(checklist, default);


        // Assert
        var res = await _eventStore.LoadAsync(checklist.Id, default);


        // Assert.That(checklistEntities, Does.Contain(checklist));
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

        var sut = new ChecklistRepo(_eventStore, _stateStore);

        // Act
        await sut.AddAsync(checklist, default);


        // Assert
        var res = await _eventStore.LoadAsync(checklist.Id, default);


        // Assert.That(checklistEntities, Does.Contain(checklist));
        Assert.That(res, Is.Empty);
    }


    // TODO: 
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

        var sut = new ChecklistRepo(_eventStore, _stateStore);
        await sut.AddAsync(oldChecklist, default);
        await _eventsContext.SaveChangesAsync();


        // Act
        var checklist = await sut.GetByIdAsync(checklistId, default);

        // Assert

        Assert.That(checklist, Is.Not.Null);
        Assert.That(checklist.DomainEvents, Has.Exactly(eventsCount).Items);
    }



    [Test]
    public async Task GetByIdAsync_State()
    {
    }

}

