using Application.Interfaces.Events;
using AutoFixture;
using TestHelperFactory;
using Infrastructure.Persistence.Data;
using Domain.Entities.Users.Events;
using Infrastructure.Persistence.Data.Exceptions;
using Domain.Entities.Users.ValueObjects;
using Infrastructure.Persistence;
using Domain.SeedWork;


namespace InfrastructureTests.DataTests;


[TestFixture]
public class EventStoreTests
{
    private readonly Fixture _fix = TestFactory.GetFixture();
    private readonly RoutineContext _context = TestFactory.GetDbContext();
    private readonly IEventSerializer _eventSerializer = TestFactory.GetEventSerializer();

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

        var aggregateId = _fix.Create<UserId>();
        var createUserEvents = _fix.Build<UserCreated>()
            .With(e => e.AggregateId, aggregateId)
            .CreateMany()
            .ToList();

        // Act
        await sut.AppendAsync(
                aggregateId: aggregateId,
                events: createUserEvents,
                expectedVersion: null,
                ct: default);
        await _context.SaveChangesAsync();

        // Assert
        var onDbEventVersions = _context.Events
            .Where(e => e.AggregateId == aggregateId)
            .Select(e => e.Version)
            .ToList();

        Assert.That(onDbEventVersions,
                Is.EquivalentTo(createUserEvents.Select(e => e.Version)));
    }



    [Test]
    public async Task AppendAsyncTest_WithExpectedVersion()
    {
        // Arrange
        IEventStore sut = new SQLiteEventStore(
                _eventSerializer,
                _context);

        var aggregateId = _fix.Create<UserId>();
        var existingEvent = _fix.Build<UserCreated>()
            .With(e => e.AggregateId, aggregateId)
            .Create();

        var dbEvent = Event.From(
                existingEvent,
                _eventSerializer.Serialize(existingEvent));

        await _context.Events.AddAsync(dbEvent);
        await _context.SaveChangesAsync();

        var createUserEvents = _fix.Build<UserCreated>()
            .With(e => e.AggregateId, aggregateId)
            .CreateMany()
            .ToList();

        // Act
        await sut.AppendAsync(
                aggregateId: aggregateId,
                events: createUserEvents,
                expectedVersion: existingEvent.Version,
                ct: default);
        await _context.SaveChangesAsync();

        // Assert
        var onDbEventVersions = _context.Events
            .Where(e => e.AggregateId == aggregateId)
            .Select(e => e.Version)
            .ToList();

        List<IDomainEvent<AggregateRootId>> allEvents = [existingEvent, .. createUserEvents];

        Assert.That(onDbEventVersions,
                Is.EquivalentTo(allEvents.Select(e => e.Version)));
    }



    [Test]
    public async Task AppendAsyncTest_ConcurrencyException()
    {
        // Arrange
        var aggregateId = _fix.Create<UserId>();

        IEventStore sut = new SQLiteEventStore(
                _eventSerializer,
                _context);

        var createUserEvents = _fix.CreateMany<UserCreated>()
            .ToList();

        var upToDateVersion = _fix.Create<int>();
        var dbEvent = Event.From(
                createUserEvents[0],
                _eventSerializer.Serialize(createUserEvents[0]));
        dbEvent.Version = upToDateVersion;

        await _context.AddAsync(dbEvent);
        await _context.SaveChangesAsync();

        var olderVersion = upToDateVersion - 1;

        // Act
        // Assert
        Assert.ThrowsAsync<ConcurrencyException>(async () =>
            await sut.AppendAsync(
                    aggregateId: aggregateId,
                    events: createUserEvents,
                    expectedVersion: olderVersion,
                    ct: default)
            );
    }


    [Test]
    public async Task LoadAsyncTest_Success()
    {
        // Arrange
        var context = TestFactory.GetDbContext();


        var aggregateId = _fix.Create<UserId>();
        var responseEvent = _fix.Build<UserCreated>()
            .With(e => e.AggregateId, aggregateId)
            .Create();


        var onDbEvent = Event.From(
                responseEvent,
                _eventSerializer.Serialize(responseEvent));
        await context.AddAsync(onDbEvent);
        await context.SaveChangesAsync();

        IEventStore sut = new SQLiteEventStore(
                _eventSerializer,
                context);

        // Act
        var res = await sut.LoadAsync(
                aggregateId: aggregateId,
                ct: default);

        // Assert
        Assert.That(res, Is.EquivalentTo([responseEvent]));
    }

    [Test]
    public async Task LoadAsyncTest_Empty()
    {
        // Arrange
        var context = TestFactory.GetDbContext();

        var aggregateId = _fix.Create<UserId>();

        IEventStore sut = new SQLiteEventStore(
                _eventSerializer,
                context);

        // Act
        var res = await sut.LoadAsync(
                aggregateId: aggregateId,
                ct: default);

        // Assert
        Assert.That(res, Has.Count.EqualTo(0));
    }


    [TestCase(0, null)]
    [TestCase(1, 1)]
    [TestCase(1, 3)]
    public async Task LoadAsyncTest_FromVersion(int fromVersion, int? toVersion)
    {
        // Arrange
        var context = TestFactory.GetDbContext();

        int versionCount = 1;

        var aggregateId = _fix.Create<UserId>();
        var responseEvents = _fix.Build<UserCreated>()
            .With(e => e.AggregateId, aggregateId)
            .With(e => e.Version, versionCount++)
            .CreateMany()
            .ToList();


        var onDbEvents = responseEvents.Select(e =>
                Event.From(e, _eventSerializer.Serialize(e)));

        await context.AddRangeAsync(onDbEvents);
        await context.SaveChangesAsync();

        IEventStore sut = new SQLiteEventStore(
                _eventSerializer,
                context);

        // Act
        var res = await sut.LoadAsync(
                aggregateId: aggregateId,
                ct: default,
                fromVersion: fromVersion,
                toVersion: toVersion);

        // Assert
        var expected = responseEvents
            .Where(e => e.Version >= fromVersion
                    && (toVersion == null || e.Version <= toVersion));

        Assert.That(res, Is.EquivalentTo(expected));
    }
}

