using System.Text.Json;
using Application.Interfaces.Events;
using AutoFixture;
using Domain.Common.ValueObjects;
using Domain.Entities.Schedules;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users.ValueObjects;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Data;
using Infrastructure.Repos;
using TestHelperFactory;

namespace InfrastructureTests.RepoTests;


[TestFixture]
public class ChecklistRepoTests
{
    private readonly Fixture _fix = TestFactory.GetFixture();

    private IEventStore _eventStore;
    private IEntityStateStore<ChecklistState, ChecklistId> _stateStore;
    private EventContext _eventContext = null!;
    private StateContext _stateContext = null!;

    [SetUp]
    public async Task SetUpAsync()
    {
        _eventContext = await TestFactory.GetEventContextAsync();
        _stateContext = await TestFactory.GetStateContextAsync();
        _eventStore = new SQLiteEventStore(TestFactory.GetEventMapper(), _eventContext);
        _stateStore = new SQLiteChecklistStateStore(_stateContext);
        _eventContext.RemoveRange(_eventContext.Events);
        _stateContext.RemoveRange(_stateContext.Checklists);
        await _eventContext.SaveChangesAsync();
        await _stateContext.SaveChangesAsync();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _eventContext.DisposeAsync();
        await _stateContext.DisposeAsync();
        TestFactory.Reset();
    }

    private async Task Commit()
    {
        await _eventContext.SaveChangesAsync();
        await _stateContext.SaveChangesAsync();
    }

    [Test]
    public async Task Add_Empty()
    {
        // Arrange
        var checklist = new Checklist();
        var checklistId = new ChecklistId(Guid.NewGuid());
        var userId = new UserId(Guid.NewGuid());
        checklist.Create(checklistId, userId);
        var domainEvents = checklist.DomainEvents;

        var sut = new ChecklistRepo(_eventStore, _stateStore);

        // Act
        await sut.Save(checklist, default);
        await Commit();


        // Assert
        var res = await _eventStore.Load(checklist.Id, default);

        Assert.That(res, Is.EquivalentTo(domainEvents));
    }


    [Test]
    public async Task Add_Events()
    {
        // Arrange
        var checklistId = new ChecklistId(Guid.NewGuid());
        var userId = new UserId(Guid.NewGuid());

        var checklist = new Checklist();
        checklist.Create(checklistId, userId);

        var domainEvents = checklist.DomainEvents;
        var sut = new ChecklistRepo(_eventStore, _stateStore);

        // Act
        await sut.Save(checklist, default);
        await Commit();

        // Assert
        var res = await _eventStore.Load(checklistId, default);
        Console.WriteLine($"res[0].AggregateId: {res[0].AggregateId.Value}");
        Assert.That(res, Has.Count.EqualTo(domainEvents.Count));
        Assert.That(res[0].AggregateId.Value, Is.EqualTo(domainEvents[0].AggregateId.Value));
        Assert.That(res[0].Timestamp, Is.EqualTo(domainEvents[0].Timestamp));
        Assert.That(res[0].Version, Is.EqualTo(domainEvents[0].Version));
    }


    [TestCase(1)]
    [TestCase(3)]
    public async Task GetById_Events(int eventsCount)
    {
        // Arrange
        var checklistId = new ChecklistId(Guid.NewGuid());

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

        await _eventStore.Append(
            oldChecklist.Id,
            oldChecklist.DomainEvents,
            oldChecklist.StoredVersion,
            default);
        await _eventContext.SaveChangesAsync();

        var sut = new ChecklistRepo(_eventStore, _stateStore);

        // Act
        var checklist = await sut.GetById(checklistId, default);

        // Assert

        Assert.That(checklist, Is.Not.Null);
        Assert.That(JsonSerializer.Serialize(checklist.State.Tasks),
                Is.EquivalentTo(JsonSerializer.Serialize(oldChecklist.State.Tasks)));
        Assert.That(checklist.State.Id, Is.EqualTo(oldChecklist.State.Id));
        Assert.That(checklist.State.Statistics, Is.EqualTo(oldChecklist.State.Statistics));
        Assert.That(checklist.State.UserId, Is.EqualTo(oldChecklist.State.UserId));
        Assert.That(checklist.DomainEvents, Is.Empty);
    }



    [TestCase(1)]
    [TestCase(3)]
    public async Task GetById_State(int eventsCount)
    {
        // Arrange
        var checklistId = new ChecklistId(Guid.NewGuid());

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

        await _stateStore.Save(oldChecklist.State, default);
        await _stateContext.SaveChangesAsync();

        var sut = new ChecklistRepo(_eventStore, _stateStore);

        // Act
        var checklist = await sut.GetById(checklistId, default);

        // Assert
        Assert.That(checklist, Is.Not.Null);
        Assert.That(checklist.State, Is.EqualTo(oldChecklist.State));
        Assert.That(checklist.DomainEvents, Is.Empty);
    }

}

