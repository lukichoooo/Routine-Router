using Attributes.GeneratorAttributes;

namespace Unit.SourceDtoGeneration;

public class EntityWithCollections
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Item> Items { get; set; } = [];
    public List<string> Tags { get; set; } = [];
    public List<NestedEntity>? OptionalItems { get; set; }
    public IEnumerable<NestedEntity> EnumerableItems { get; set; } = [];
}

public class Item
{
    public Guid Id { get; set; }
    public string Value { get; set; } = string.Empty;
}

[GenerateDto(typeof(Item))]
public partial class ItemDto;

public class NestedEntity
{
    public Guid Id { get; set; }
    public string Data { get; set; } = string.Empty;
}

[GenerateDto(typeof(NestedEntity))]
public partial class NestedEntityDto;

[GenerateDto(typeof(EntityWithCollections))]
[Map(nameof(EntityWithCollections.Items), typeof(ItemDto))]
[Map(nameof(EntityWithCollections.EnumerableItems), typeof(NestedEntityDto))]
public partial class EntityWithCollectionsDto;


public class CollectionTests
{
    [Test]
    public void From_ListProperty_CreatesCopy()
    {
        var entity = new EntityWithCollections
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Items = new List<Item>
            {
                new() { Id = Guid.NewGuid(), Value = "Item1" },
                new() { Id = Guid.NewGuid(), Value = "Item2" }
            }
        };

        var dto = EntityWithCollectionsDto.From(entity);

        Assert.That(dto.Items, Is.Not.Null);
        Assert.That(dto.Items, Has.Count.EqualTo(2));
        Assert.That(ReferenceEquals(dto.Items, entity.Items), Is.False);
    }

    [Test]
    public void From_ListProperty_CopiesElements()
    {
        var itemId1 = Guid.NewGuid();
        var itemId2 = Guid.NewGuid();
        
        var entity = new EntityWithCollections
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Items = new List<Item>
            {
                new() { Id = itemId1, Value = "Item1" },
                new() { Id = itemId2, Value = "Item2" }
            }
        };

        var dto = EntityWithCollectionsDto.From(entity);

        Assert.That(dto.Items[0].Id, Is.EqualTo(itemId1));
        Assert.That(dto.Items[0].Value, Is.EqualTo("Item1"));
        Assert.That(dto.Items[1].Id, Is.EqualTo(itemId2));
        Assert.That(dto.Items[1].Value, Is.EqualTo("Item2"));
    }

    [Test]
    public void From_NullableList_NullStaysNull()
    {
        var entity = new EntityWithCollections
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            OptionalItems = null
        };

        var dto = EntityWithCollectionsDto.From(entity);

        Assert.That(dto.OptionalItems, Is.Null);
    }

    [Test]
    public void From_NullableList_WithValues_CreatesCopy()
    {
        var entity = new EntityWithCollections
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            OptionalItems = new List<NestedEntity>
            {
                new() { Id = Guid.NewGuid(), Data = "Data1" }
            }
        };

        var dto = EntityWithCollectionsDto.From(entity);

        Assert.That(dto.OptionalItems, Is.Not.Null);
        Assert.That(dto.OptionalItems, Has.Count.EqualTo(1));
        Assert.That(ReferenceEquals(dto.OptionalItems, entity.OptionalItems), Is.False);
    }

    [Test]
    public void From_EmptyList_StaysEmpty()
    {
        var entity = new EntityWithCollections
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Items = new List<Item>()
        };

        var dto = EntityWithCollectionsDto.From(entity);

        Assert.That(dto.Items, Is.Not.Null);
        Assert.That(dto.Items, Has.Count.EqualTo(0));
    }

    [Test]
    public void From_ListWithMapping_ConvertsElements()
    {
        var itemId = Guid.NewGuid();
        
        var entity = new EntityWithCollections
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Items = new List<Item>
            {
                new() { Id = itemId, Value = "TestValue" }
            }
        };

        var dto = EntityWithCollectionsDto.From(entity);

        Assert.That(dto.Items[0], Is.TypeOf<ItemDto>());
        Assert.That(dto.Items[0].Id, Is.EqualTo(itemId));
        Assert.That(dto.Items[0].Value, Is.EqualTo("TestValue"));
    }

    [Test]
    public void From_NonMappedList_CreatesCopy()
    {
        var entity = new EntityWithCollections
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Tags = new List<string> { "tag1", "tag2", "tag3" }
        };

        var dto = EntityWithCollectionsDto.From(entity);

        Assert.That(ReferenceEquals(dto.Tags, entity.Tags), Is.False);
        Assert.That(dto.Tags, Is.EquivalentTo(entity.Tags));
    }

    [Test]
    public void From_Enumerable_ConvertsToList()
    {
        var entity = new EntityWithCollections
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            EnumerableItems = new List<NestedEntity>
            {
                new() { Id = Guid.NewGuid(), Data = "Data1" }
            }
        };

        var dto = EntityWithCollectionsDto.From(entity);

        Assert.That(dto.EnumerableItems, Is.TypeOf<List<NestedEntityDto>>());
        Assert.That(dto.EnumerableItems[0].Data, Is.EqualTo("Data1"));
    }

    [Test]
    public void From_ModifiedDto_DoesNotAffectEntity()
    {
        var entity = new EntityWithCollections
        {
            Id = Guid.NewGuid(),
            Name = "Original",
            Items = new List<Item>
            {
                new() { Id = Guid.NewGuid(), Value = "OriginalItem" }
            }
        };

        var dto = EntityWithCollectionsDto.From(entity);

        dto.Name = "Modified";
        dto.Items[0].Value = "ModifiedItem";

        Assert.That(entity.Name, Is.EqualTo("Original"));
        Assert.That(entity.Items[0].Value, Is.EqualTo("OriginalItem"));
    }
}
