using Domain.SeedWork;
using Generated.EventMapper;
using Infrastructure.Persistence.Data;

namespace Unit.EventMapperGeneration;

public sealed class TestEntityId : EntityId
{
    public TestEntityId(Guid value) : base(value) { }
}

public sealed record SimpleEvent(
    TestEntityId AggregateId,
    int Version,
    DateTimeOffset Timestamp,
    string Name
) : BaseDomainEvent<TestEntityId>;

public sealed record EventWithMultiplePayload(
    TestEntityId AggregateId,
    int Version,
    DateTimeOffset Timestamp,
    string Name,
    int Count,
    bool IsActive
) : BaseDomainEvent<TestEntityId>;

public sealed record EventWithValueObjectPayload(
    TestEntityId AggregateId,
    int Version,
    DateTimeOffset Timestamp,
    TestValueObject ValueObj
) : BaseDomainEvent<TestEntityId>;

public record TestValueObject(string Value, int Number);

public class SimpleTests
{
    [Test]
    public void FromPayload_WithSimpleEvent_CreatesDomainEvent()
    {
        var dbEvent = new Event
        {
            AggregateId = Guid.NewGuid(),
            AggregateIdType = nameof(TestEntityId),
            Version = 1,
            Timestamp = DateTimeOffset.UtcNow,
            EventType = nameof(SimpleEvent),
            Payload = """{"Name": "TestName"}"""
        };

        var result = EventMapper.FromDbEvent(dbEvent) as SimpleEvent;

        Assert.That(result!.AggregateId.Value, Is.EqualTo(dbEvent.AggregateId));
        Assert.That(result.Version, Is.EqualTo(dbEvent.Version));
        Assert.That(result.Timestamp, Is.EqualTo(dbEvent.Timestamp));
        Assert.That(result.Name, Is.EqualTo("TestName"));
    }

    [Test]
    public void FromPayload_WithMultiplePayloadProps_CreatesDomainEvent()
    {
        var dbEvent = new Event
        {
            AggregateId = Guid.NewGuid(),
            AggregateIdType = nameof(TestEntityId),
            Version = 2,
            Timestamp = DateTimeOffset.UtcNow,
            EventType = nameof(EventWithMultiplePayload),
            Payload = """{"Name": "TestName", "Count": 5, "IsActive": true}"""
        };

        var result = EventMapper.FromDbEvent(dbEvent) as EventWithMultiplePayload;

        Assert.That(result!.AggregateId.Value, Is.EqualTo(dbEvent.AggregateId));
        Assert.That(result.Version, Is.EqualTo(dbEvent.Version));
        Assert.That(result.Name, Is.EqualTo("TestName"));
        Assert.That(result.Count, Is.EqualTo(5));
        Assert.That(result.IsActive, Is.True);
    }

    [Test]
    public void ToPayload_WithSimpleEvent_ReturnsJsonString()
    {
        var domainEvent = new SimpleEvent(
            new TestEntityId(Guid.NewGuid()),
            1,
            DateTimeOffset.UtcNow,
            "TestName"
        );

        var result = EventMapper.ToPayload(domainEvent);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Does.Contain("TestName"));
        Assert.That(result, Does.Not.Contain("AggregateId"));
        Assert.That(result, Does.Not.Contain("Version"));
        Assert.That(result, Does.Not.Contain("Timestamp"));
    }

    [Test]
    public void ToPayload_WithMultiplePayloadProps_ReturnsValidJson()
    {
        var domainEvent = new EventWithMultiplePayload(
            new TestEntityId(Guid.NewGuid()),
            1,
            DateTimeOffset.UtcNow,
            "TestName",
            10,
            true
        );

        var result = EventMapper.ToPayload(domainEvent);

        Assert.That(result, Does.Contain("TestName"));
        Assert.That(result, Does.Contain("10"));
        Assert.That(result, Does.Contain("true"));
        Assert.That(result, Does.Not.Contain("AggregateId"));
        Assert.That(result, Does.Not.Contain("Version"));
        Assert.That(result, Does.Not.Contain("Timestamp"));
    }

    [Test]
    public void RoundTrip_FromPayload_ToPayload_PreservesData()
    {
        var originalEvent = new SimpleEvent(
            new TestEntityId(Guid.NewGuid()),
            1,
            DateTimeOffset.UtcNow,
            "RoundTripTest"
        );

        var dbEvent = new Event
        {
            AggregateId = originalEvent.AggregateId.ToGuid(),
            AggregateIdType = nameof(TestEntityId),
            Version = originalEvent.Version,
            Timestamp = originalEvent.Timestamp,
            EventType = nameof(SimpleEvent),
            Payload = EventMapper.ToPayload(originalEvent)
        };

        var result = EventMapper.FromDbEvent(dbEvent) as SimpleEvent;

        Assert.That(result!.Name, Is.EqualTo(originalEvent.Name));
    }
}
