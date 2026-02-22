using Application.Interfaces.Events;
using AutoFixture;
using Domain.Common.ValueObjects;
using Domain.Entities.Users;
using Domain.Entities.Users.ValueObjects;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Data;
using Infrastructure.Repos;
using TestHelperFactory;

namespace InfrastructureTests.RepoTests;

[TestFixture]
public class UserRepoTests
{
    private readonly Fixture _fix = TestFactory.GetFixture();

    private IEventStore _eventStore = TestFactory.GetEventStore();
    private IEntityStateStore<UserState, UserId> _stateStore = TestFactory.GetUserStateStore();
    private readonly EventsContext _eventsContext = TestFactory.GetEventsContext();
    private readonly EntitiesContext _entitiesContext = TestFactory.GetEntitiesContext();

    [SetUp]
    public async Task SetUpAsync()
    {
        _eventStore = new SQLiteEventStore(TestFactory.GetEventMapper(), _eventsContext);
        _stateStore = new SQLiteStateStore<UserState, UserId>(_entitiesContext);
        _eventsContext.RemoveRange(_eventsContext.Events);
        _entitiesContext.RemoveRange(_entitiesContext.Checklists);
        await _eventsContext.SaveChangesAsync();
        await _entitiesContext.SaveChangesAsync();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _eventsContext.DisposeAsync();
        await _entitiesContext.DisposeAsync();
        TestFactory.Reset();
    }

    private async Task Commit()
    {
        await _eventsContext.SaveChangesAsync();
        await _entitiesContext.SaveChangesAsync();
    }

    [Test]
    public async Task Add_Empty()
    {
        // Arrange
        var user = new User();
        var userId = new UserId(Guid.NewGuid());
        user.Create(userId, _fix.Create<Name>(), _fix.Create<PasswordHash>());
        var domainEvents = user.DomainEvents;

        var sut = new UserRepo(_eventStore, _stateStore);

        // Act
        await sut.Add(user, default);
        await Commit();


        // Assert
        var res = await _eventStore.Load(user.Id, default);
        Assert.That(res, Is.EquivalentTo(domainEvents));
    }


    [Test]
    public async Task Add_Events()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var user = new User();
        user.Create(userId, _fix.Create<Name>(), _fix.Create<PasswordHash>());

        var domainEvents = user.DomainEvents;
        var sut = new UserRepo(_eventStore, _stateStore);

        // Act
        await sut.Add(user, default);
        await Commit();

        // Assert
        var res = await _eventStore.Load(userId, default);
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
        var userId = new UserId(Guid.NewGuid());

        var oldUser = new User();
        var name = _fix.Create<Name>();
        var pass = _fix.Create<PasswordHash>();
        oldUser.Create(userId, name, pass);

        for (int i = 1; i < eventsCount; ++i)
        {
            oldUser.Verify(name, pass);
        }

        var sut = new UserRepo(_eventStore, _stateStore);
        await sut.Add(oldUser, default);
        await _eventsContext.SaveChangesAsync();

        // Act
        var user = await sut.GetById(userId, default);

        // Assert
        Assert.That(user, Is.Not.Null);
        Assert.That(user.State, Is.EqualTo(oldUser.State));
        Assert.That(user.DomainEvents, Is.Empty);
    }



    [TestCase(1)]
    [TestCase(3)]
    public async Task GetById_State(int eventsCount)
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());

        var oldUser = new User();
        var name = _fix.Create<Name>();
        var pass = _fix.Create<PasswordHash>();
        oldUser.Create(userId, name, pass);

        for (int i = 1; i < eventsCount; ++i)
        {
            oldUser.Verify(name, pass);
        }


        var sut = new UserRepo(_eventStore, _stateStore);
        await sut.Add(oldUser, default);
        await _entitiesContext.SaveChangesAsync();

        // Act
        var user = await sut.GetById(userId, default);

        // Assert
        Assert.That(user, Is.Not.Null);
        Assert.That(user.State, Is.EqualTo(oldUser.State));
        Assert.That(user.DomainEvents, Is.Empty);
    }


}

