using Attributes.GeneratorAttributes;

namespace Unit.SourceDtoGeneration;

public class EntityWithDefaults
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "DefaultName";
    public int Count { get; set; } = 42;
    public bool IsEnabled { get; set; } = true;
    public decimal Price { get; set; } = 9.99m;
}

[GenerateDto(typeof(EntityWithDefaults))]
public partial class EntityWithDefaultsDto;

public class EntityWithSpecialTypes
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public TimeSpan Duration { get; set; }
    public Guid? OptionalId { get; set; }
    public string? OptionalName { get; set; }
    public int? OptionalCount { get; set; }
    public List<string> Tags { get; set; } = [];
}

[GenerateDto(typeof(EntityWithSpecialTypes))]
public partial class EntityWithSpecialTypesDto;

public class EntityWithNestedNullables
{
    public Guid Id { get; set; }
    public NestedClass? Nested { get; set; }
    public List<NestedClass?>? NullableList { get; set; }
}

public class NestedClass
{
    public string Value { get; set; } = string.Empty;
}

[GenerateDto(typeof(EntityWithNestedNullables))]
public partial class EntityWithNestedNullablesDto;


public class EdgeCaseTests
{
    [Test]
    public void From_EntityWithDefaults_preservesDefaults()
    {
        var entity = new EntityWithDefaults();

        var dto = EntityWithDefaultsDto.From(entity);

        Assert.That(dto.Name, Is.EqualTo("DefaultName"));
        Assert.That(dto.Count, Is.EqualTo(42));
        Assert.That(dto.IsEnabled, Is.EqualTo(true));
        Assert.That(dto.Price, Is.EqualTo(9.99m));
    }

    [Test]
    public void From_EntityWithDefaults_OverwritesWithEntityValues()
    {
        var entity = new EntityWithDefaults
        {
            Id = Guid.NewGuid(),
            Name = "CustomName",
            Count = 100,
            IsEnabled = false,
            Price = 50.0m
        };

        var dto = EntityWithDefaultsDto.From(entity);

        Assert.That(dto.Id, Is.EqualTo(entity.Id));
        Assert.That(dto.Name, Is.EqualTo("CustomName"));
        Assert.That(dto.Count, Is.EqualTo(100));
        Assert.That(dto.IsEnabled, Is.EqualTo(false));
        Assert.That(dto.Price, Is.EqualTo(50.0m));
    }

    [Test]
    public void From_SpecialTypes_MapsCorrectly()
    {
        var now = DateTime.Now;
        var duration = TimeSpan.FromHours(2);

        var entity = new EntityWithSpecialTypes
        {
            Id = Guid.NewGuid(),
            CreatedAt = now,
            Duration = duration,
            OptionalId = Guid.NewGuid(),
            OptionalName = "Optional",
            OptionalCount = 10,
            Tags = new List<string> { "tag1", "tag2" }
        };

        var dto = EntityWithSpecialTypesDto.From(entity);

        Assert.That(dto.Id, Is.EqualTo(entity.Id));
        Assert.That(dto.CreatedAt, Is.EqualTo(entity.CreatedAt));
        Assert.That(dto.Duration, Is.EqualTo(entity.Duration));
        Assert.That(dto.OptionalId, Is.EqualTo(entity.OptionalId));
        Assert.That(dto.OptionalName, Is.EqualTo(entity.OptionalName));
        Assert.That(dto.OptionalCount, Is.EqualTo(entity.OptionalCount));
        Assert.That(dto.Tags, Is.EquivalentTo(entity.Tags));
    }

    [Test]
    public void From_NullableTypes_WithNullValues()
    {
        var entity = new EntityWithSpecialTypes
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Duration = TimeSpan.Zero,
            OptionalId = null,
            OptionalName = null,
            OptionalCount = null,
            Tags = null!
        };

        var dto = EntityWithSpecialTypesDto.From(entity);

        Assert.That(dto.OptionalId, Is.Null);
        Assert.That(dto.OptionalName, Is.Null);
        Assert.That(dto.OptionalCount, Is.Null);
        Assert.That(dto.Tags, Is.Null);
    }

    [Test]
    public void From_NestedNullableClass_NullStaysNull()
    {
        var entity = new EntityWithNestedNullables
        {
            Id = Guid.NewGuid(),
            Nested = null
        };

        var dto = EntityWithNestedNullablesDto.From(entity);

        Assert.That(dto.Nested, Is.Null);
    }

    [Test]
    public void From_NestedNullableClass_WithValue_MapsCorrectly()
    {
        var entity = new EntityWithNestedNullables
        {
            Id = Guid.NewGuid(),
            Nested = new NestedClass { Value = "TestValue" }
        };

        var dto = EntityWithNestedNullablesDto.From(entity);

        Assert.That(dto.Nested, Is.Not.Null);
        Assert.That(dto.Nested.Value, Is.EqualTo("TestValue"));
    }

    [Test]
    public void From_NullableListWithNullElements_HandledCorrectly()
    {
        var entity = new EntityWithNestedNullables
        {
            Id = Guid.NewGuid(),
            NullableList = new List<NestedClass?> { new(), null, new() { Value = "Test" } }
        };

        var dto = EntityWithNestedNullablesDto.From(entity);

        Assert.That(dto.NullableList, Is.Not.Null);
        Assert.That(dto.NullableList, Has.Count.EqualTo(3));
    }

    [Test]
    public void Dto_NewInstance_HasDefaultConstructor()
    {
        var dto = new EntityWithDefaultsDto();

        Assert.That(dto, Is.Not.Null);
    }

    [Test]
    public void Dto_IsIndependent_FromEntity()
    {
        var entity = new EntityWithDefaults
        {
            Id = Guid.NewGuid(),
            Name = "Original"
        };

        var dto = EntityWithDefaultsDto.From(entity);

        dto.Name = "Modified";

        Assert.That(entity.Name, Is.EqualTo("Original"));
        Assert.That(dto.Name, Is.EqualTo("Modified"));
    }

    [Test]
    public void From_Guid_MapsCorrectly()
    {
        var id = Guid.NewGuid();

        var entity = new EntityWithDefaults
        {
            Id = id
        };

        var dto = EntityWithDefaultsDto.From(entity);

        Assert.That(dto.Id, Is.EqualTo(id));
    }

    [Test]
    public void From_DateTime_MapsCorrectly()
    {
        var now = DateTime.Now;

        var entity = new EntityWithSpecialTypes
        {
            Id = Guid.NewGuid(),
            CreatedAt = now
        };

        var dto = EntityWithSpecialTypesDto.From(entity);

        Assert.That(dto.CreatedAt, Is.EqualTo(now));
    }

    [Test]
    public void From_TimeSpan_MapsCorrectly()
    {
        var duration = TimeSpan.FromMinutes(30);

        var entity = new EntityWithSpecialTypes
        {
            Id = Guid.NewGuid(),
            Duration = duration
        };

        var dto = EntityWithSpecialTypesDto.From(entity);

        Assert.That(dto.Duration, Is.EqualTo(duration));
    }

    [Test]
    public void From_Decimal_MapsCorrectly()
    {
        var price = 123.45m;

        var entity = new EntityWithDefaults
        {
            Id = Guid.NewGuid(),
            Price = price
        };

        var dto = EntityWithDefaultsDto.From(entity);

        Assert.That(dto.Price, Is.EqualTo(price));
    }
}
