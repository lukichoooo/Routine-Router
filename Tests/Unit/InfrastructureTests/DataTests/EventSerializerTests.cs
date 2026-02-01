using AutoFixture;
using AutoFixture.Kernel;
using Domain.Entities.Users.Events;
using Domain.SeedWork;
using FixtureProvider;
using Infrastructure.Persistence.Data;
using Microsoft.Extensions.Logging;

namespace InfrastructureTests.DataTests;


[TestFixture]
public class EventSerializerTests
{
    private readonly Fixture _fix = FixtureFactory.GetFixture();

    private IEventSerializer EventSerializer =>
        new EventSerializer();


    [Test]
    public void SerializeEvent_DeserializeEvent_Tests()
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });
        var logger = loggerFactory.CreateLogger<EventSerializerTests>();

        var assembly = typeof(UserCreated).Assembly;
        var eventTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface
                && typeof(IDomainEvent<AggregateRootId>).IsAssignableFrom(t));

        var events = eventTypes
            .Select(t => _fix.Create(t, new SpecimenContext(_fix)))
            .ToList();

        foreach (IDomainEvent<AggregateRootId> e in events)
        {
            string serialized = EventSerializer.Serialize(e);
            var deserialized = EventSerializer.Deserialize(serialized, e.GetType().Name);
            Assert.That(e, Is.EqualTo(deserialized));

            logger.LogInformation("Event Type: {0},\nFull: {1}\n",
                    e.GetType().Name,
                    e.GetType().FullName);

            logger.LogInformation("Event Json: {0},\n", serialized);
        }
    }

}

