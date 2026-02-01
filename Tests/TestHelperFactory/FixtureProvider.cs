using AutoFixture;
using Domain.Entities.Schedules.ValueObjects;
using Infrastructure.Persistence;
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
                (byte)rnd.Next(0, 101),
                (byte)rnd.Next(0, 101),
                (byte)rnd.Next(0, 101),
                (byte)rnd.Next(0, 101),
                (byte)rnd.Next(0, 101),
                (byte)rnd.Next(0, 101)));

        return fix;
    }

    public static RoutineContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<RoutineContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        return new RoutineContext(options);
    }


    public static IEventSerializer GetEventSerializer()
        => new EventSerializer();
}

