using Application.Interfaces.Data;
using Application.Interfaces.Events;
using AutoFixture;
using Domain.Entities.Schedules;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users;
using Domain.Entities.Users.ValueObjects;
using Infrastructure.EventPublishing;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Data.Serializer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace TestHelperFactory;


// <summary>
//  static factory using singleton pattern
//  </summary>
public static class TestFactory
{
    private static Fixture? _fixture;
    private static EventContext? _eventContext;
    private static StateContext? _stateContext;
    private static IJsonEventMapper? _eventMapper;
    private static IEventStore? _eventStore;
    private static IEntityStateStore<ChecklistState, ChecklistId>? _checklistStateStore;
    private static IEntityStateStore<UserState, UserId>? _userStateStore;

    public static void Reset()
    {
        _eventContext?.Dispose();
        _eventContext = null;

        _stateContext?.Dispose();
        _stateContext = null;

        _eventStore = null;
        _checklistStateStore = null;
        _userStateStore = null;
    }

    public static Fixture GetFixture()
    {
        _fixture ??= CreateFixture();
        return _fixture;
    }

    private static Fixture CreateFixture()
    {
        Fixture fix = new();
        var rnd = new Random();
        fix.Behaviors.Add(new OmitOnRecursionBehavior(3));

        fix.Register<Rating>(() => new(
                (byte)rnd.Next(0, 100),
                (byte)rnd.Next(0, 100),
                (byte)rnd.Next(0, 100),
                (byte)rnd.Next(0, 100),
                (byte)rnd.Next(0, 100),
                (byte)rnd.Next(0, 100)));

        fix.Register<Guid>(Guid.NewGuid);

        return fix;
    }

    // DB
    public static async Task<EventContext> GetEventContextAsync()
        => _eventContext ??= await CreateEventContextAsync();

    private static async Task<EventContext> CreateEventContextAsync()
    {
        var options = new DbContextOptionsBuilder<EventContext>()
            .UseInMemoryDatabase("TestDb")
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new EventContext(options);
    }

    public static async Task<StateContext> GetStateContextAsync()
        => _stateContext ??= await CreateStateContextAsync();

    private static async Task<StateContext> CreateStateContextAsync()
    {
        var options = new DbContextOptionsBuilder<StateContext>()
            .UseInMemoryDatabase("TestDb")
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new StateContext(options);
    }


    public static async Task<IUnitOfWork> GetTestUnitOfWorkAsync(IDomainEventDispatcher eventDispatcher)
        => new TestUnitOfWork(
                new LoggerFactory().CreateLogger<TestUnitOfWork>(),
                await GetEventContextAsync(),
                await GetStateContextAsync(),
                eventDispatcher
                );


    // serializer

    public static IJsonEventMapper GetEventMapper()
        => _eventMapper ??= new JsonEventMapper();

    public static async Task<IEventStore> GetEventStoreAsync()
        => _eventStore ??= new SQLiteEventStore(GetEventMapper(), await GetEventContextAsync());


    // Entity State Stores
    public static async Task<IEntityStateStore<ChecklistState, ChecklistId>> GetChecklistStateStoreAsync()
        => _checklistStateStore ??= new SQLiteChecklistStateStore(await GetStateContextAsync());

    public static async Task<IEntityStateStore<UserState, UserId>> GetUserStateStoreAsync()
        => _userStateStore ??= new SQLiteUserStateStore(await GetStateContextAsync());
}

