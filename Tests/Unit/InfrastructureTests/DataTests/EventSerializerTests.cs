using AutoFixture;
using Domain.Entities.Users.Events;
using Domain.SeedWork;
using Infrastructure.Persistence.Data;
using Microsoft.Extensions.Logging;

namespace InfrastructureTests.DataTests;


[TestFixture]
public class EventSerializerTests
{
    private readonly Fixture _fix = new();

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _fix.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    private IEventSerializer EventSerializer =>
        new EventSerializer(new LoggerFactory().CreateLogger<EventSerializer>());

    [Test]
    public void SerializeEvent()
    {
        var e = _fix.Create<UserCreated>();
        string serialized = EventSerializer.Serialize(e);
        IDomainEvent deserialized = EventSerializer.Deserialize(serialized, e.GetType().Name);
        Assert.That(e, Is.EqualTo(deserialized));
    }
}

