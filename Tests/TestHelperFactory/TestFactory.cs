using Application.Interfaces.Events;
using AutoFixture;
using Domain.Entities.Schedules;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users;
using Domain.Entities.Users.ValueObjects;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Data.Serializer;
using Microsoft.EntityFrameworkCore;

namespace TestHelperFactory;


// <summary>
//  static factory using singleton pattern
//  </summary>
public static class TestFactory
{
    private static Fixture? _fixture;
    private static EventsContext? _eventsContext;
    private static EntitiesContext? _entitiesContext;
    private static IJsonEventMapper? _eventMapper;
    private static IEventStore? _eventStore;
    private static IEntityStateStore<ChecklistState, ChecklistId>? _checklistStateStore;
    private static IEntityStateStore<UserState, UserId>? _userStateStore;

    public static void Reset()
    {
        _eventsContext?.Dispose();
        _eventsContext = null;

        _entitiesContext?.Dispose();
        _entitiesContext = null;

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

    public static EventsContext GetEventsContext()
        => _eventsContext ??= CreateEventsContext();

    private static EventsContext CreateEventsContext()
    {
        var options = new DbContextOptionsBuilder<EventsContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new EventsContext(options);
    }


    public static EntitiesContext GetEntitiesContext()
        => _entitiesContext ??= CreateEntitiesContext();

    private static EntitiesContext CreateEntitiesContext()
    {
        var options = new DbContextOptionsBuilder<EntitiesContext>()
            .UseInMemoryDatabase("EntitiesTestDb")
            .Options;
        return new EntitiesContext(options);
    }


    public static IJsonEventMapper GetEventMapper()
        => _eventMapper ??= new JsonEventMapper();

    public static IEventStore GetEventStore()
        => _eventStore ??= new SQLiteEventStore(GetEventMapper(), GetEventsContext());


    // Entity State Stores
    public static IEntityStateStore<ChecklistState, ChecklistId> GetChecklistStateStore()
        => _checklistStateStore ??= new SQLiteStateStore<ChecklistState, ChecklistId>(GetEntitiesContext());

    public static IEntityStateStore<UserState, UserId> GetUserStateStore()
        => _userStateStore ??= new SQLiteStateStore<UserState, UserId>(GetEntitiesContext());
}

