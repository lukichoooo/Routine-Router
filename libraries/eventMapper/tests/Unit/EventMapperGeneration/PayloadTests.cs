using System.Text.Json.Nodes;
using Domain.SeedWork;
using Generated.EventMapper;
using Infrastructure.Persistence.Data;

namespace Unit.EventMapperGeneration;

public sealed record EventWithListPayload(
    TestEntityId AggregateId,
    int Version,
    DateTimeOffset Timestamp,
    List<string> Items
) : BaseDomainEvent<TestEntityId>;

public sealed record EventWithNestedObject(
    TestEntityId AggregateId,
    int Version,
    DateTimeOffset Timestamp,
    NestedData Data
) : BaseDomainEvent<TestEntityId>;

public record NestedData(string Name, int Value);

public sealed record EventWithGuidPayload(
    TestEntityId AggregateId,
    int Version,
    DateTimeOffset Timestamp,
    Guid UserId,
    Guid ItemId
) : BaseDomainEvent<TestEntityId>;

public class PayloadTests
{
    [Test]
    public void ToPayload_WithList_SerializesToJsonArray()
    {
        var domainEvent = new EventWithListPayload(
            new TestEntityId(Guid.NewGuid()),
            1,
            DateTimeOffset.UtcNow,
            new List<string> { "Item1", "Item2", "Item3" }
        );

        var result = EventMapper.ToPayload(domainEvent);

        var jsonNode = JsonNode.Parse(result);
        Assert.That(jsonNode?["Items"], Is.Not.Null);
        Assert.That(jsonNode!["Items"]!.AsArray(), Has.Count.EqualTo(3));
    }

    [Test]
    public void FromPayload_WithList_DeserializesCorrectly()
    {
        var dbEvent = new Event
        {
            AggregateId = Guid.NewGuid(),
            AggregateIdType = nameof(TestEntityId),
            Version = 1,
            Timestamp = DateTimeOffset.UtcNow,
            EventType = nameof(EventWithListPayload),
            Payload = @"{""Items"": [""A"", ""B"", ""C""]}"
        };

        var result = EventMapper.FromDbEvent(dbEvent) as EventWithListPayload;

        Assert.That(result!.Items, Has.Count.EqualTo(3));
        Assert.That(result.Items, Is.EquivalentTo(["A", "B", "C"]));
    }

    [Test]
    public void ToPayload_WithNestedObject_SerializesCorrectly()
    {
        var domainEvent = new EventWithNestedObject(
            new TestEntityId(Guid.NewGuid()),
            1,
            DateTimeOffset.UtcNow,
            new NestedData("NestedName", 42)
        );

        var result = EventMapper.ToPayload(domainEvent);

        var jsonNode = JsonNode.Parse(result);
        Assert.That(jsonNode?["Data"], Is.Not.Null);
        Assert.That(jsonNode!["Data"]!["Name"]?.GetValue<string>(), Is.EqualTo("NestedName"));
        Assert.That(jsonNode["Data"]!["Value"]?.GetValue<int>(), Is.EqualTo(42));
    }

    [Test]
    public void FromPayload_WithNestedObject_DeserializesCorrectly()
    {
        var dbEvent = new Event
        {
            AggregateId = Guid.NewGuid(),
            AggregateIdType = nameof(TestEntityId),
            Version = 1,
            Timestamp = DateTimeOffset.UtcNow,
            EventType = nameof(EventWithNestedObject),
            Payload = @"{""Data"": {""Name"": ""TestData"", ""Value"": 100}}"
        };

        var result = EventMapper.FromDbEvent(dbEvent) as EventWithNestedObject;

        Assert.That(result!.Data, Is.Not.Null);
        Assert.That(result.Data.Name, Is.EqualTo("TestData"));
        Assert.That(result.Data.Value, Is.EqualTo(100));
    }

    [Test]
    public void ToPayload_WithGuidProperties_SerializesToString()
    {
        var userId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        var domainEvent = new EventWithGuidPayload(
            new TestEntityId(Guid.NewGuid()),
            1,
            DateTimeOffset.UtcNow,
            userId,
            itemId
        );

        var result = EventMapper.ToPayload(domainEvent);

        var jsonNode = JsonNode.Parse(result);
        Assert.That(jsonNode?["UserId"]?.GetValue<string>(), Is.EqualTo(userId.ToString()));
        Assert.That(jsonNode?["ItemId"]?.GetValue<string>(), Is.EqualTo(itemId.ToString()));
    }

    [Test]
    public void FromPayload_WithGuidProperties_DeserializesToGuid()
    {
        var userId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        var dbEvent = new Event
        {
            AggregateId = Guid.NewGuid(),
            AggregateIdType = nameof(TestEntityId),
            Version = 1,
            Timestamp = DateTimeOffset.UtcNow,
            EventType = nameof(EventWithGuidPayload),
            Payload = @"{""UserId"": """ + userId + @""", ""ItemId"": """ + itemId + @"""}"
        };

        var result = EventMapper.FromDbEvent(dbEvent) as EventWithGuidPayload;

        Assert.That(result!.UserId, Is.EqualTo(userId));
        Assert.That(result.ItemId, Is.EqualTo(itemId));
    }

    [Test]
    public void RoundTrip_ListPayload_PreservesData()
    {
        var original = new EventWithListPayload(
            new TestEntityId(Guid.NewGuid()),
            1,
            DateTimeOffset.UtcNow,
            new List<string> { "X", "Y", "Z" }
        );

        var dbEvent = new Event
        {
            AggregateId = original.AggregateId.ToGuid(),
            AggregateIdType = nameof(TestEntityId),
            Version = original.Version,
            Timestamp = original.Timestamp,
            EventType = nameof(EventWithListPayload),
            Payload = EventMapper.ToPayload(original)
        };

        var result = EventMapper.FromDbEvent(dbEvent) as EventWithListPayload;

        Assert.That(result!.Items, Is.EquivalentTo(original.Items));
    }

    [Test]
    public void RoundTrip_NestedObject_PreservesData()
    {
        var original = new EventWithNestedObject(
            new TestEntityId(Guid.NewGuid()),
            1,
            DateTimeOffset.UtcNow,
            new NestedData("Original", 999)
        );

        var dbEvent = new Event
        {
            AggregateId = original.AggregateId.ToGuid(),
            AggregateIdType = nameof(TestEntityId),
            Version = original.Version,
            Timestamp = original.Timestamp,
            EventType = nameof(EventWithNestedObject),
            Payload = EventMapper.ToPayload(original)
        };

        var result = EventMapper.FromDbEvent(dbEvent) as EventWithNestedObject;

        Assert.That(result!.Data.Name, Is.EqualTo(original.Data.Name));
        Assert.That(result.Data.Value, Is.EqualTo(original.Data.Value));
    }
}
