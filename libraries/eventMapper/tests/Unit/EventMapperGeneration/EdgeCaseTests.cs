using System.Text.Json.Nodes;
using EventMapperAbstractions.Events;
using Generated.EventMapper;

namespace Unit.EventMapperGeneration;

public sealed record EventWithNullableString(
    TestEntityId AggregateId,
    int Version,
    DateTimeOffset Timestamp,
    string? OptionalName
) : IEvent<TestEntityId>;

public sealed record EventWithAllBaseProps(
    TestEntityId AggregateId,
    int Version,
    DateTimeOffset Timestamp
) : IEvent<TestEntityId>;

public sealed record EventWithDecimal(
    TestEntityId AggregateId,
    int Version,
    DateTimeOffset Timestamp,
    decimal Amount,
    double Rate
) : IEvent<TestEntityId>;

public sealed record EventWithDateTime(
    TestEntityId AggregateId,
    int Version,
    DateTimeOffset Timestamp,
    DateTime ScheduledTime,
    DateTimeOffset OccurredAt
) : IEvent<TestEntityId>;

public class EdgeCaseTests
{
    [Test]
    public void FromPayload_WithNullStringProperty_SetsNull()
    {
        var dbEvent = new Event
        {
            AggregateId = Guid.NewGuid(),
            AggregateIdType = nameof(TestEntityId),
            Version = 1,
            Timestamp = DateTimeOffset.UtcNow,
            EventType = nameof(EventWithNullableString),
            Payload = """{"OptionalName": null}"""
        };

        var result = EventMapper.FromDbEvent(dbEvent) as EventWithNullableString;

        Assert.That(result!.OptionalName, Is.Null);
    }

    [Test]
    public void FromPayload_WithEmptyString_SetsEmpty()
    {
        var dbEvent = new Event
        {
            AggregateId = Guid.NewGuid(),
            AggregateIdType = nameof(TestEntityId),
            Version = 1,
            Timestamp = DateTimeOffset.UtcNow,
            EventType = nameof(EventWithNullableString),
            Payload = """{"OptionalName": ""}"""
        };

        var result = EventMapper.FromDbEvent(dbEvent) as EventWithNullableString;

        Assert.That(result!.OptionalName, Is.EqualTo(string.Empty));
    }

    [Test]
    public void ToPayload_WithNullString_SerializesAsNull()
    {
        var domainEvent = new EventWithNullableString(
            new TestEntityId(Guid.NewGuid()),
            1,
            DateTimeOffset.UtcNow,
            null
        );

        var result = EventMapper.ToPayload(domainEvent);

        Assert.That(result, Does.Contain("null"));
    }

    [Test]
    public void FromPayload_WithOnlyBaseProps_CreatesEvent()
    {
        var aggregateId = Guid.NewGuid();
        var timestamp = DateTimeOffset.UtcNow;

        var dbEvent = new Event
        {
            AggregateId = aggregateId,
            AggregateIdType = nameof(TestEntityId),
            Version = 5,
            Timestamp = timestamp,
            EventType = nameof(EventWithAllBaseProps),
            Payload = "{}"
        };

        var result = EventMapper.FromDbEvent(dbEvent) as EventWithAllBaseProps;

        Assert.That(result!.AggregateId.ToGuid(), Is.EqualTo(aggregateId));
        Assert.That(result.Version, Is.EqualTo(5));
        Assert.That(result.Timestamp, Is.EqualTo(timestamp));
    }

    [Test]
    public void ToPayload_WithOnlyBaseProps_ReturnsEmptyJson()
    {
        var domainEvent = new EventWithAllBaseProps(
            new TestEntityId(Guid.NewGuid()),
            1,
            DateTimeOffset.UtcNow
        );

        var result = EventMapper.ToPayload(domainEvent);

        var jsonNode = JsonNode.Parse(result);
        Assert.That(jsonNode, Is.Not.Null);
        Assert.That(jsonNode!.AsObject(), Is.Empty);
    }

    [Test]
    public void FromPayload_WithDecimal_SerializesCorrectly()
    {
        var dbEvent = new Event
        {
            AggregateId = Guid.NewGuid(),
            AggregateIdType = nameof(TestEntityId),
            Version = 1,
            Timestamp = DateTimeOffset.UtcNow,
            EventType = nameof(EventWithDecimal),
            Payload = """{"Amount": 123.45, "Rate": 0.055}"""
        };

        var result = EventMapper.FromDbEvent(dbEvent) as EventWithDecimal;

        Assert.That(result!.Amount, Is.EqualTo(123.45m));
        Assert.That(result.Rate, Is.EqualTo(0.055).Within(0.0001));
    }

    [Test]
    public void ToPayload_WithDecimal_SerializesCorrectly()
    {
        var domainEvent = new EventWithDecimal(
            new TestEntityId(Guid.NewGuid()),
            1,
            DateTimeOffset.UtcNow,
            999.99m,
            0.25
        );

        var result = EventMapper.ToPayload(domainEvent);

        Assert.That(result, Does.Contain("999.99"));
        Assert.That(result, Does.Contain("0.25"));
    }

    [Test]
    public void FromPayload_WithDateTimeProperties_DeserializesCorrectly()
    {
        var scheduledTime = new DateTime(2025, 6, 15, 10, 30, 0, DateTimeKind.Utc);
        var occurredAt = new DateTimeOffset(2025, 6, 15, 10, 30, 0, TimeSpan.Zero);

        var payload = @"{""ScheduledTime"": """ + scheduledTime.ToString("O") + @""", ""OccurredAt"": """ + occurredAt.ToString("O") + @"""}";

        var dbEvent = new Event
        {
            AggregateId = Guid.NewGuid(),
            AggregateIdType = nameof(TestEntityId),
            Version = 1,
            Timestamp = DateTimeOffset.UtcNow,
            EventType = nameof(EventWithDateTime),
            Payload = payload
        };

        var result = EventMapper.FromDbEvent(dbEvent) as EventWithDateTime;

        Assert.That(result!.ScheduledTime, Is.EqualTo(scheduledTime));
        Assert.That(result.OccurredAt, Is.EqualTo(occurredAt));
    }

    [Test]
    public void ToPayload_WithDateTimeProperties_SerializesCorrectly()
    {
        var scheduledTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var occurredAt = new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);

        var domainEvent = new EventWithDateTime(
            new TestEntityId(Guid.NewGuid()),
            1,
            DateTimeOffset.UtcNow,
            scheduledTime,
            occurredAt
        );

        var result = EventMapper.ToPayload(domainEvent);

        Assert.That(result, Does.Contain("ScheduledTime"));
        Assert.That(result, Does.Contain("OccurredAt"));
    }

    [Test]
    public void RoundTrip_NullableString_PreservesNull()
    {
        var original = new EventWithNullableString(
            new TestEntityId(Guid.NewGuid()),
            1,
            DateTimeOffset.UtcNow,
            null
        );

        var dbEvent = new Event
        {
            AggregateId = original.AggregateId.ToGuid(),
            AggregateIdType = nameof(TestEntityId),
            Version = original.Version,
            Timestamp = original.Timestamp,
            EventType = nameof(EventWithNullableString),
            Payload = EventMapper.ToPayload(original)
        };

        var result = EventMapper.FromDbEvent(dbEvent) as EventWithNullableString;

        Assert.That(result!.OptionalName, Is.Null);
    }

    [Test]
    public void RoundTrip_Decimal_PreservesPrecision()
    {
        var original = new EventWithDecimal(
            new TestEntityId(Guid.NewGuid()),
            1,
            DateTimeOffset.UtcNow,
            123456.789m,
            0.123456789
        );

        var dbEvent = new Event
        {
            AggregateId = original.AggregateId.ToGuid(),
            AggregateIdType = nameof(TestEntityId),
            Version = original.Version,
            Timestamp = original.Timestamp,
            EventType = nameof(EventWithDecimal),
            Payload = EventMapper.ToPayload(original)
        };

        var result = EventMapper.FromDbEvent(dbEvent) as EventWithDecimal;

        Assert.That(result!.Amount, Is.EqualTo(123456.789m));
        Assert.That(result.Rate, Is.EqualTo(0.123456789).Within(0.0000001));
    }

    [Test]
    public void FromPayload_WithLargePayload_DoesNotThrow()
    {
        var largeString = new string('x', 10000);
        var payload = @"{""OptionalName"": """ + largeString + @"""}";
        var dbEvent = new Event
        {
            AggregateId = Guid.NewGuid(),
            AggregateIdType = nameof(TestEntityId),
            Version = 1,
            Timestamp = DateTimeOffset.UtcNow,
            EventType = nameof(EventWithNullableString),
            Payload = payload
        };

        var result = EventMapper.FromDbEvent(dbEvent) as EventWithNullableString;

        Assert.That(result!.OptionalName, Is.EqualTo(largeString));
    }
}
