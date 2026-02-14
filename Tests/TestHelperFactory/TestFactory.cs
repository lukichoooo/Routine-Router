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


public static class TestFactory
{
    public static Fixture GetFixture()
    {
        Fixture fix = new();
        var rnd = new Random();
        fix.Behaviors.Add(new OmitOnRecursionBehavior());

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
    {
        var options = new DbContextOptionsBuilder<EventsContext>()
            .UseInMemoryDatabase("EventsTestDb") // might need change
            .Options;
        return new(options);
    }


    public static EntitiesContext GetEntitiesContext()
    {
        var options = new DbContextOptionsBuilder<EntitiesContext>()
            .UseInMemoryDatabase("EntitiesTestDb") // might need change
            .Options;
        return new(options);
    }


    public static IJsonEventMapper GetEventMapper()
        => new JsonEventMapper();

    public static IEventStore GetEventStore()
        => new SQLiteEventStore(GetEventMapper(), GetEventsContext());


    // Entity State Stores
    public static IEntityStateStore<ChecklistState, ChecklistId> GetChecklistStateStore()
        => new SQLiteStateStore<ChecklistState, ChecklistId>(GetEntitiesContext());

    public static IEntityStateStore<UserState, UserId> GetUserStateStore()
        => new SQLiteStateStore<UserState, UserId>(GetEntitiesContext());
}

