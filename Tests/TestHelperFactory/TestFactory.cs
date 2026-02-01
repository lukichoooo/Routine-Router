using AutoFixture;
using Domain.Entities.Schedules.ValueObjects;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Data;
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

        return fix;
    }

    public static EventsContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<EventsContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        return new EventsContext(options);
    }


    public static IEventSerializer GetEventSerializer()
        => new EventSerializer();
}

