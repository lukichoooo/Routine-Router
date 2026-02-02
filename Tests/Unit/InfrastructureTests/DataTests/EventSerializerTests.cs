using AutoFixture;
using AutoFixture.Kernel;
using Domain.SeedWork;
using TestHelperFactory;
using Infrastructure.Persistence.Data;
using Microsoft.Extensions.Logging;

namespace InfrastructureTests.DataTests;


[TestFixture]
public class EventSerializerTests
{
    private readonly Fixture _fix = TestFactory.GetFixture();

    private IEventSerializer EventSerializer =>
        new JsonEventSerializer();

    private ILogger GetLogger()
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            // builder.AddConsole(); // Comment out to not log
            builder.SetMinimumLevel(LogLevel.Debug);
        });
        return loggerFactory.CreateLogger<EventSerializerTests>();
    }


    [Test]
    public void SerializeEvent_DeserializeEvent_Tests()
    {
        var logger = GetLogger();

        var assembly = typeof(IDomainEvent).Assembly;
        var eventTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface
                && typeof(IDomainEvent).IsAssignableFrom(t));

        var events = eventTypes
            .Select(t => _fix.Create(t, new SpecimenContext(_fix)))
            .ToList();

        foreach (IDomainEvent e in events)
        {
            string serialized = EventSerializer.Serialize(e);
            var deserialized = EventSerializer.Deserialize(serialized, e.GetType().Name);
            Assert.That(e, Is.EqualTo(deserialized));

            logger.LogInformation("Event Type: {0},\nFull: {1}\n",
                    e.GetType().Name,
                    e.GetType().FullName);

            logger.LogInformation("Event Json: {0},\n", serialized);
        }

        logger.LogInformation("Tested Events: {0}\n\n{1}\n",
                events.Count,
                events.ConvertAll(e => e.GetType().Name + "\n")
                .Order());
    }

}

