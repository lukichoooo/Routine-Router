using Attributes.GeneratorAttributes;

namespace Unit.SourceDtoGeneration;

public class EntityWithNullableProps
{
    public Guid Id { get; set; }
    public string? NullableString { get; set; }
    public int? NullableInt { get; set; }
    public Guid? NullableGuid { get; set; }
    public DateTime? NullableDateTime { get; set; }
    public object? NullableObject { get; set; }
}

[GenerateDto(typeof(EntityWithNullableProps))]
public partial class EntityWithNullablePropsDto;

public class EntityWithMappedNullable
{
    public Guid Id { get; set; }
    public InnerEntity? Inner { get; set; }
    public List<InnerEntity?>? InnerList { get; set; }
}

public class InnerEntity
{
    public Guid Id { get; set; }
    public string Value { get; set; } = string.Empty;
}

[GenerateDto(typeof(InnerEntity))]
public partial class InnerEntityDto;

[GenerateDto(typeof(EntityWithMappedNullable))]
[Map(nameof(EntityWithMappedNullable.Inner), typeof(InnerEntityDto))]
[Map(nameof(EntityWithMappedNullable.InnerList), typeof(InnerEntityDto))]
public partial class EntityWithMappedNullableDto;


public class NullHandlingTests
{
    [Test]
    public void From_NullableString_NullStaysNull()
    {
        var entity = new EntityWithNullableProps
        {
            Id = Guid.NewGuid(),
            NullableString = null
        };

        var dto = EntityWithNullablePropsDto.From(entity);

        Assert.That(dto.NullableString, Is.Null);
    }

    [Test]
    public void From_NullableString_WithValue_MapsCorrectly()
    {
        var entity = new EntityWithNullableProps
        {
            Id = Guid.NewGuid(),
            NullableString = "TestValue"
        };

        var dto = EntityWithNullablePropsDto.From(entity);

        Assert.That(dto.NullableString, Is.EqualTo("TestValue"));
    }

    [Test]
    public void From_NullableInt_NullStaysNull()
    {
        var entity = new EntityWithNullableProps
        {
            Id = Guid.NewGuid(),
            NullableInt = null
        };

        var dto = EntityWithNullablePropsDto.From(entity);

        Assert.That(dto.NullableInt, Is.Null);
    }

    [Test]
    public void From_NullableInt_WithValue_MapsCorrectly()
    {
        var entity = new EntityWithNullableProps
        {
            Id = Guid.NewGuid(),
            NullableInt = 42
        };

        var dto = EntityWithNullablePropsDto.From(entity);

        Assert.That(dto.NullableInt, Is.EqualTo(42));
    }

    [Test]
    public void From_NullableGuid_NullStaysNull()
    {
        var entity = new EntityWithNullableProps
        {
            Id = Guid.NewGuid(),
            NullableGuid = null
        };

        var dto = EntityWithNullablePropsDto.From(entity);

        Assert.That(dto.NullableGuid, Is.Null);
    }

    [Test]
    public void From_NullableGuid_WithValue_MapsCorrectly()
    {
        var guid = Guid.NewGuid();
        
        var entity = new EntityWithNullableProps
        {
            Id = Guid.NewGuid(),
            NullableGuid = guid
        };

        var dto = EntityWithNullablePropsDto.From(entity);

        Assert.That(dto.NullableGuid, Is.EqualTo(guid));
    }

    [Test]
    public void From_NullableDateTime_NullStaysNull()
    {
        var entity = new EntityWithNullableProps
        {
            Id = Guid.NewGuid(),
            NullableDateTime = null
        };

        var dto = EntityWithNullablePropsDto.From(entity);

        Assert.That(dto.NullableDateTime, Is.Null);
    }

    [Test]
    public void From_NullableDateTime_WithValue_MapsCorrectly()
    {
        var dateTime = DateTime.Now;
        
        var entity = new EntityWithNullableProps
        {
            Id = Guid.NewGuid(),
            NullableDateTime = dateTime
        };

        var dto = EntityWithNullablePropsDto.From(entity);

        Assert.That(dto.NullableDateTime, Is.EqualTo(dateTime));
    }

    [Test]
    public void From_NullableObject_NullStaysNull()
    {
        var entity = new EntityWithNullableProps
        {
            Id = Guid.NewGuid(),
            NullableObject = null
        };

        var dto = EntityWithNullablePropsDto.From(entity);

        Assert.That(dto.NullableObject, Is.Null);
    }

    [Test]
    public void From_NullableObject_WithValue_MapsCorrectly()
    {
        var obj = new object();
        
        var entity = new EntityWithNullableProps
        {
            Id = Guid.NewGuid(),
            NullableObject = obj
        };

        var dto = EntityWithNullablePropsDto.From(entity);

        Assert.That(dto.NullableObject, Is.EqualTo(obj));
    }

    [Test]
    public void From_MappedNullableClass_NullStaysNull()
    {
        var entity = new EntityWithMappedNullable
        {
            Id = Guid.NewGuid(),
            Inner = null
        };

        var dto = EntityWithMappedNullableDto.From(entity);

        Assert.That(dto.Inner, Is.Null);
    }

    [Test]
    public void From_MappedNullableClass_WithValue_MapsCorrectly()
    {
        var entity = new EntityWithMappedNullable
        {
            Id = Guid.NewGuid(),
            Inner = new InnerEntity { Id = Guid.NewGuid(), Value = "Test" }
        };

        var dto = EntityWithMappedNullableDto.From(entity);

        Assert.That(dto.Inner, Is.Not.Null);
        Assert.That(dto.Inner, Is.TypeOf<InnerEntityDto>());
        Assert.That(dto.Inner.Value, Is.EqualTo("Test"));
    }

    [Test]
    public void From_MappedNullableList_NullStaysNull()
    {
        var entity = new EntityWithMappedNullable
        {
            Id = Guid.NewGuid(),
            InnerList = null
        };

        var dto = EntityWithMappedNullableDto.From(entity);

        Assert.That(dto.InnerList, Is.Null);
    }

    [Test]
    public void From_MappedNullableList_WithValues_MapsCorrectly()
    {
        var entity = new EntityWithMappedNullable
        {
            Id = Guid.NewGuid(),
            InnerList = new List<InnerEntity?>
            {
                new() { Id = Guid.NewGuid(), Value = "Item1" },
                new() { Id = Guid.NewGuid(), Value = "Item2" }
            }
        };

        var dto = EntityWithMappedNullableDto.From(entity);

        Assert.That(dto.InnerList, Is.Not.Null);
        Assert.That(dto.InnerList, Has.Count.EqualTo(2));
        Assert.That(dto.InnerList[0], Is.TypeOf<InnerEntityDto>());
    }

    [Test]
    public void From_AllNullableTypes_MixedValues()
    {
        var guid = Guid.NewGuid();
        var dateTime = DateTime.Now;
        
        var entity = new EntityWithNullableProps
        {
            Id = Guid.NewGuid(),
            NullableString = "Hello",
            NullableInt = 100,
            NullableGuid = guid,
            NullableDateTime = dateTime,
            NullableObject = new object()
        };

        var dto = EntityWithNullablePropsDto.From(entity);

        Assert.That(dto.NullableString, Is.EqualTo("Hello"));
        Assert.That(dto.NullableInt, Is.EqualTo(100));
        Assert.That(dto.NullableGuid, Is.EqualTo(guid));
        Assert.That(dto.NullableDateTime, Is.EqualTo(dateTime));
        Assert.That(dto.NullableObject, Is.Not.Null);
    }

    [Test]
    public void From_AllNullableTypes_AllNull()
    {
        var entity = new EntityWithNullableProps
        {
            Id = Guid.NewGuid(),
            NullableString = null,
            NullableInt = null,
            NullableGuid = null,
            NullableDateTime = null,
            NullableObject = null
        };

        var dto = EntityWithNullablePropsDto.From(entity);

        Assert.That(dto.NullableString, Is.Null);
        Assert.That(dto.NullableInt, Is.Null);
        Assert.That(dto.NullableGuid, Is.Null);
        Assert.That(dto.NullableDateTime, Is.Null);
        Assert.That(dto.NullableObject, Is.Null);
    }
}
