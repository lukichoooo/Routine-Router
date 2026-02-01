using AutoFixture;
using Domain.Entities.Schedules.ValueObjects;

namespace FixtureProvider;


public static class FixtureFactory
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
}

