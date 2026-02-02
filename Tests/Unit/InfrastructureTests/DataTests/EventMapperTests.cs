using AutoFixture;
using AutoFixture.Kernel;
using Domain.SeedWork;
using TestHelperFactory;
using Microsoft.Extensions.Logging;
using Infrastructure.Persistence.Data.Serializer;
using Infrastructure.Persistence.Data;

namespace InfrastructureTests.DataTests;


[TestFixture]
public class EventMapperTests
{
    private readonly Fixture _fix = TestFactory.GetFixture();

    private ILogger GetLogger()
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            // builder.AddConsole(); // Comment out to not log
            builder.SetMinimumLevel(LogLevel.Debug);
        });
        return loggerFactory.CreateLogger<EventMapperTests>();
    }


    [Test]
    public void ToDomainEvent_And_ToPayload_Tests()
    {
        var logger = GetLogger();

        var assembly = typeof(IDomainEvent).Assembly;
        var eventTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface
                && typeof(IDomainEvent).IsAssignableFrom(t));

        var events = eventTypes
            .Select(t => _fix.Create(t, new SpecimenContext(_fix)))
            .ToList();

        var sut = new JsonEventMapper();

        foreach (IDomainEvent domainEvent in events)
        {
            string payload = sut.ToPayload(domainEvent);
            var dbEvent = Event.From(domainEvent, payload);

            var deserializedDomainEvent = sut.ToDomainEvent(dbEvent);

            Assert.That(domainEvent, Is.EqualTo(deserializedDomainEvent));
            AssertProperties(domainEvent, deserializedDomainEvent);

            logger.LogInformation("DomainEvent: {0},\nFull: {1}\n",
                    domainEvent.GetType().Name,
                    domainEvent.GetType().FullName);

            logger.LogInformation("Payload: {0},\n", payload);
        }

        logger.LogInformation("Tested Events: {0}\n\n{1}\n",
                events.Count,
                events.ConvertAll(e => e.GetType().Name + "\n")
                .Order());

        Assert.That(events, Is.Not.Empty);
    }


    // helper

    private void AssertProperties(IDomainEvent domainEvent, IDomainEvent deserializedDomainEvent)
    {
        Assert.That(domainEvent.GetType(), Is.EqualTo(deserializedDomainEvent.GetType()));
        Assert.That(domainEvent.AggregateId, Is.EqualTo(deserializedDomainEvent.AggregateId));
        Assert.That(domainEvent.Version, Is.EqualTo(deserializedDomainEvent.Version));
    }

}

