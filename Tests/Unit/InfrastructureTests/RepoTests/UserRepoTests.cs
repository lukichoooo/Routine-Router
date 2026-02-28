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

    private IEventStore _eventStore;
    private IEntityStateStore<UserState, UserId> _stateStore;
    private EventContext _eventContext;
    private StateContext _stateContext;

    [SetUp]
    public async Task SetUpAsync()
    {
        _eventContext = await TestFactory.GetEventContextAsync();
        _stateContext = await TestFactory.GetStateContextAsync();
        _eventStore = new SQLiteEventStore(TestFactory.GetEventMapper(), _eventContext);
        _stateStore = new SQLiteStateStore<UserState, UserId>(_stateContext);
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
        var user = new User();
        var userId = new UserId(Guid.NewGuid());
        user.Create(userId, _fix.Create<Name>(), _fix.Create<PasswordHash>());
        var domainEvents = user.DomainEvents;

        var sut = new UserRepo(_eventStore, _stateStore);

        // Act
        await sut.Save(user, default);
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
        await sut.Save(user, default);
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
        await sut.Save(oldUser, default);
        await _eventContext.SaveChangesAsync();

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
        await sut.Save(oldUser, default);
        await _stateContext.SaveChangesAsync();

        // Act
        var user = await sut.GetById(userId, default);

        // Assert
        Assert.That(user, Is.Not.Null);
        Assert.That(user.State, Is.EqualTo(oldUser.State));
        Assert.That(user.DomainEvents, Is.Empty);
    }


}

