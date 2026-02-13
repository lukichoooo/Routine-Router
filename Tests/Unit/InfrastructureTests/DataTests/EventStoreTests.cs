using Application.Interfaces.Events;
using AutoFixture;
using TestHelperFactory;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Data.Exceptions;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Data.Serializer;
using Domain.Entities.Users;
using Domain.Common.ValueObjects;


namespace InfrastructureTests.DataTests;


[TestFixture]
public class EventStoreTests
{
    private readonly Fixture _fix = TestFactory.GetFixture();
    private readonly EventsContext _context = TestFactory.GetEventsContext();
    private readonly IJsonEventMapper _eventSerializer = TestFactory.GetEventMapper();

    [OneTimeTearDown]
    public async Task OneTimeTearDownAsync()
    {
        await _context.DisposeAsync();
    }

    [Test]
    public async Task AppendAsyncTest_Success()
    {
        // Arrange
        IEventStore sut = new SQLiteEventStore(
                _eventSerializer,
                _context);

        var user = new User();
        user.Create(_fix.Create<UserId>(), _fix.Create<Name>(), _fix.Create<PasswordHash>());
        var userEvents = user.DomainEvents;

        // Act
        await sut.AppendAsync(
                aggregateId: user.Id,
                events: user.DomainEvents,
                expectedVersion: null,
                ct: default);
        await _context.SaveChangesAsync();

        // Assert
        var onDbEventVersions = _context.Events
            .Where(e => e.AggregateId == user.Id.Value)
            .Select(e => e.Version)
            .ToList();

        Assert.That(onDbEventVersions,
                Is.EquivalentTo(userEvents.Select(e => e.Version)));
    }



    [Test]
    public async Task AppendAsyncTest_WithExpectedVersion()
    {
        // Arrange
        IEventStore sut = new SQLiteEventStore(
                _eventSerializer,
                _context);

        var aggregateId = _fix.Create<UserId>();
        var user = new User();
        user.Create(aggregateId, _fix.Create<Name>(), _fix.Create<PasswordHash>());
        var domainEvents = user.DomainEvents;

        var userHistory = await sut.LoadAsync(
                aggregateId: aggregateId,
                ct: default
        );
        var dbUser = new User(userHistory);
        user.Update(_fix.Create<Name>(), _fix.Create<PasswordHash>());
        var newEvents = dbUser.DomainEvents;

        // Act
        await sut.AppendAsync(
                aggregateId: aggregateId,
                events: newEvents,
                expectedVersion: dbUser.StoredVersion,
                ct: default);

        // Assert
        var onDbEvents = _context.Events
            .Where(e => e.AggregateId == aggregateId.Value)
            .ToList();

        List<IDomainEvent> allEvents = [.. domainEvents, .. newEvents];

        Assert.That(onDbEvents.Select(e => e.Version),
                Is.EquivalentTo(allEvents.Select(e => e.Version)));
    }



    [Test]
    public async Task AppendAsyncTest_ConcurrencyException()
    {
        // Arrange
        IEventStore sut = new SQLiteEventStore(
                _eventSerializer,
                _context);

        var aggregateId = _fix.Create<UserId>();
        var user = new User();
        user.Create(aggregateId, _fix.Create<Name>(), _fix.Create<PasswordHash>());
        var domainEvents = user.DomainEvents;

        var dbEvent = Event.From(
                domainEvents.First(),
                _eventSerializer.ToPayload(domainEvents.First()));

        await _context.Events.AddAsync(dbEvent);
        await _context.SaveChangesAsync();

        var userHistory = await sut.LoadAsync(
                aggregateId: aggregateId,
                ct: default
        );
        var dbUser = new User(userHistory);
        user.Update(_fix.Create<Name>(), _fix.Create<PasswordHash>());
        var newEvents = dbUser.DomainEvents;


        // Act

        var wrongVersion = 100;

        Assert.ThrowsAsync<ConcurrencyException>(async () =>
                await sut.AppendAsync(
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

