using AutoFixture;
using TestHelperFactory;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Data.Exceptions;
using Domain.Entities.Users.ValueObjects;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Data.Serializer;
using Domain.Entities.Users;
using Domain.Common.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Domain.Entities.Users.Events;


namespace InfrastructureTests.DataTests;


[TestFixture]
public class EventStoreTests
{
    private readonly Fixture _fix = TestFactory.GetFixture();
    private readonly EventsContext _eventsContext = TestFactory.GetEventsContext();
    private readonly IJsonEventMapper _eventSerializer = TestFactory.GetEventMapper();

    [OneTimeTearDown]
    public async Task OneTimeTearDownAsync()
    {
        await _eventsContext.DisposeAsync();
    }

    [TearDown]
    public void TearDown()
    {
        _eventsContext.Events.RemoveRange(_eventsContext.Events);
        _eventsContext.SaveChanges();
        _eventsContext.ChangeTracker.Clear();
    }

    [Test]
    public async Task AppendTest_Success()
    {
        // Arrange
        var sut = new SQLiteEventStore(
                _eventSerializer,
                _eventsContext);

        var user = new User();
        user.Create(_fix.Create<UserId>(), _fix.Create<Name>(), _fix.Create<PasswordHash>());
        var userEvents = user.DomainEvents;

        // Act
        await sut.Append(
                aggregateId: user.Id,
                events: user.DomainEvents,
                expectedVersion: null,
                ct: default);
        await _eventsContext.SaveChangesAsync();

        // Assert
        var onDbEventVersions = _eventsContext.Events
            .Where(e => e.AggregateId == user.Id.Value)
            .Select(e => e.Version)
            .ToList();

        Assert.That(onDbEventVersions,
                Is.EquivalentTo(userEvents.Select(e => e.Version)));
    }



    [Test]
    public async Task AppendTest_WithExpectedVersion()
    {
        // Arrange
        var sut = new SQLiteEventStore(_eventSerializer, _eventsContext);

        var ogUser = new User();
        ogUser.Create(new UserId(Guid.NewGuid()), _fix.Create<Name>(), _fix.Create<PasswordHash>());

        // Act
        await sut.Append(
                aggregateId: ogUser.Id,
                events: ogUser.DomainEvents,
                expectedVersion: ogUser.StoredVersion,
                ct: default);

        Console.WriteLine("---ogUser Events: ");
        foreach (var e in ogUser.DomainEvents)
            Console.WriteLine(e.GetType().Name);

        await _eventsContext.SaveChangesAsync();


        // Arrange
        var userHistory = await _eventsContext.Events
            .AsNoTracking()
            .Where(e => e.AggregateId == ogUser.Id.Value)
            .OrderBy(e => e.Version)
            .Select(e => _eventSerializer.ToDomainEvent(e))
            .ToListAsync();
        Assert.That(userHistory, Has.Exactly(1).InstanceOf<UserCreated>());

        // After line 103, before line 106
        var loadedUser = new User(userHistory);

        Assert.That(loadedUser.Id, Is.EqualTo(ogUser.Id));
        Assert.That(loadedUser.StoredVersion, Is.EqualTo(ogUser.Version));

        loadedUser.Update(_fix.Create<Name>(), _fix.Create<PasswordHash>());

        // Act
        await sut.Append(
                aggregateId: loadedUser.Id,
                events: loadedUser.DomainEvents,
                expectedVersion: loadedUser.StoredVersion,
                ct: default);
        Console.WriteLine("---dbUser Events: ");
        foreach (var e in loadedUser.DomainEvents)
            Console.WriteLine(e.GetType().Name);
        await _eventsContext.SaveChangesAsync();


        // Assert
        var onDbEvents = await _eventsContext.Events
            .Where(e => e.AggregateId == loadedUser.Id.Value)
            .ToListAsync();

        Console.WriteLine("---Stored Events: ");
        foreach (var e in onDbEvents)
            Console.WriteLine(e.EventType);

        Assert.That(onDbEvents.Select(e => e.Version),
                Is.EquivalentTo([1, 2]));
    }



    [Test]
    public async Task AppendTest_ConcurrencyException()
    {
        // Arrange
        var sut = new SQLiteEventStore(
                _eventSerializer,
                _eventsContext);

        var aggregateId = _fix.Create<UserId>();
        var user = new User();
        user.Create(aggregateId, _fix.Create<Name>(), _fix.Create<PasswordHash>());
        var domainEvents = user.DomainEvents;

        var dbEvent = Event.From(
                domainEvents[0],
                _eventSerializer.ToPayload(domainEvents[0]));

        await _eventsContext.Events.AddAsync(dbEvent);
        await _eventsContext.SaveChangesAsync();

        var userHistory = await sut.Load(
                aggregateId: aggregateId,
                ct: default
        );
        var dbUser = new User(userHistory);
        user.Update(_fix.Create<Name>(), _fix.Create<PasswordHash>());
        var newEvents = dbUser.DomainEvents;


        // Act

        var wrongVersion = 100;

        Assert.ThrowsAsync<ConcurrencyException>(async () =>
                await sut.Append(
                    aggregateId: aggregateId,
                    events: newEvents,
                    expectedVersion: wrongVersion,
                    ct: default)
                );
    }

    //
    // [Test]
    // public async Task LoadAsyncTest_Success()
    // {
    //     // Arrange
    //     var context = TestFactory.GetEventsContext();
    //
    //
    //     var aggregateId = _fix.Create<UserId>();
    //     var responseEvent = _fix.Build<UserCreated>()
    //         .With(e => e.AggregateId, aggregateId)
    //         .Create();
    //
    //
    //     var onDbEvent = Event.From(
    //             responseEvent,
    //             _eventSerializer.ToPayload(responseEvent));
    //     await context.AddAsync(onDbEvent);
    //     await context.SaveChangesAsync();
    //
    //     IEventStore sut = new SQLiteEventStore(
    //             _eventSerializer,
    //             context);
    //
    //     // Act
    //     var res = await sut.LoadAsync(
    //             aggregateId: aggregateId,
    //             ct: default);
    //
    //     // Assert
    //     Assert.That(res, Is.EquivalentTo([responseEvent]));
    // }
    //
    // [Test]
    // public async Task LoadAsyncTest_Empty()
    // {
    //     // Arrange
    //     var context = TestFactory.GetEventsContext();
    //
    //     var aggregateId = _fix.Create<UserId>();
    //
    //     IEventStore sut = new SQLiteEventStore(
    //             _eventSerializer,
    //             context);
    //
    //     // Act
    //     var res = await sut.LoadAsync(
    //             aggregateId: aggregateId,
    //             ct: default);
    //
    //     // Assert
    //     Assert.That(res, Has.Count.EqualTo(0));
    // }
    //
    //
    // [TestCase(0, null)]
    // [TestCase(1, 1)]
    // [TestCase(1, 3)]
    // public async Task LoadAsyncTest_FromVersion(int fromVersion, int? toVersion)
    // {
    //     // Arrange
    //     var context = TestFactory.GetEventsContext();
    //
    //     int versionCount = 1;
    //
    //     var aggregateId = _fix.Create<UserId>();
    //     var responseEvents = _fix.Build<UserCreated>()
    //         .With(e => e.AggregateId, aggregateId)
    //         .With(e => e.Version, versionCount++)
    //         .CreateMany()
    //         .ToList();
    //
    //
    //     var onDbEvents = responseEvents.Select(e =>
    //             Event.From(e, _eventSerializer.ToPayload(e)));
    //
    //     await context.AddRangeAsync(onDbEvents);
    //     await context.SaveChangesAsync();
    //
    //     IEventStore sut = new SQLiteEventStore(
    //             _eventSerializer,
    //             context);
    //
    //     // Act
    //     var res = await sut.LoadAsync(
    //             aggregateId: aggregateId,
    //             ct: default,
    //             fromVersion: fromVersion,
    //             toVersion: toVersion);
    //
    //     // Assert
    //     var expected = responseEvents
    //         .Where(e => e.Version >= fromVersion
    //                 && (toVersion == null || e.Version <= toVersion));
    //
    //     Assert.That(res, Is.EquivalentTo(expected));
    // }
}

