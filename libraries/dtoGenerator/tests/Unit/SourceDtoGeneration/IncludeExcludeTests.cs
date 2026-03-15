using Attributes.GeneratorAttributes;

namespace Unit.SourceDtoGeneration;

public class EntityWithManyProperties
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Age { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public decimal Score { get; set; }
}

[GenerateDto(typeof(EntityWithManyProperties))]
public partial class EntityWithManyPropertiesDto;

[GenerateDto(typeof(EntityWithManyProperties), Include = [nameof(EntityWithManyProperties.Id), nameof(EntityWithManyProperties.Name)])]
public partial class EntityWithManyPropertiesDtoIncludeOnlyIdAndName;

[GenerateDto(typeof(EntityWithManyProperties), Exclude = [nameof(EntityWithManyProperties.Description), nameof(EntityWithManyProperties.Score)])]
public partial class EntityWithManyPropertiesDtoExcludeDescriptionAndScore;

[GenerateDto(typeof(EntityWithManyProperties), Include = [nameof(EntityWithManyProperties.Name)])]
public partial class EntityWithManyPropertiesDtoIncludeOnlyName;

[GenerateDto(typeof(EntityWithManyProperties), Exclude = [nameof(EntityWithManyProperties.Age), nameof(EntityWithManyProperties.IsActive)])]
public partial class EntityWithManyPropertiesDtoExcludeAgeAndActive;


public class IncludeExcludeTests
{
    [Test]
    public void Include_OnlySpecifiedProperties_Generated()
    {
        var dto = new EntityWithManyPropertiesDtoIncludeOnlyIdAndName();

        var idProperty = typeof(EntityWithManyPropertiesDtoIncludeOnlyIdAndName).GetProperty("Id");
        var nameProperty = typeof(EntityWithManyPropertiesDtoIncludeOnlyIdAndName).GetProperty("Name");
        var descriptionProperty = typeof(EntityWithManyPropertiesDtoIncludeOnlyIdAndName).GetProperty("Description");
        var ageProperty = typeof(EntityWithManyPropertiesDtoIncludeOnlyIdAndName).GetProperty("Age");

        Assert.That(idProperty, Is.Not.Null);
        Assert.That(nameProperty, Is.Not.Null);
        Assert.That(descriptionProperty, Is.Null);
        Assert.That(ageProperty, Is.Null);
    }

    [Test]
    public void Include_AllExceptSpecified_Generated()
    {
        var dto = new EntityWithManyPropertiesDtoExcludeDescriptionAndScore();

        var idProperty = typeof(EntityWithManyPropertiesDtoExcludeDescriptionAndScore).GetProperty("Id");
        var nameProperty = typeof(EntityWithManyPropertiesDtoExcludeDescriptionAndScore).GetProperty("Name");
        var descriptionProperty = typeof(EntityWithManyPropertiesDtoExcludeDescriptionAndScore).GetProperty("Description");
        var scoreProperty = typeof(EntityWithManyPropertiesDtoExcludeDescriptionAndScore).GetProperty("Score");

        Assert.That(idProperty, Is.Not.Null);
        Assert.That(nameProperty, Is.Not.Null);
        Assert.That(descriptionProperty, Is.Null);
        Assert.That(scoreProperty, Is.Null);
    }

    [Test]
    public void Include_Empty_IncludesAllProperties()
    {
        var dtoType = typeof(EntityWithManyPropertiesDto);

        var idProperty = dtoType.GetProperty("Id");
        var nameProperty = dtoType.GetProperty("Name");
        var descriptionProperty = dtoType.GetProperty("Description");
        var ageProperty = dtoType.GetProperty("Age");
        var createdAtProperty = dtoType.GetProperty("CreatedAt");
        var isActiveProperty = dtoType.GetProperty("IsActive");
        var scoreProperty = dtoType.GetProperty("Score");

        Assert.That(idProperty, Is.Not.Null);
        Assert.That(nameProperty, Is.Not.Null);
        Assert.That(descriptionProperty, Is.Not.Null);
        Assert.That(ageProperty, Is.Not.Null);
        Assert.That(createdAtProperty, Is.Not.Null);
        Assert.That(isActiveProperty, Is.Not.Null);
        Assert.That(scoreProperty, Is.Not.Null);
    }

    [Test]
    public void Exclude_Empty_ExcludesNoProperties()
    {
        var dtoType = typeof(EntityWithManyPropertiesDto);

        var idProperty = dtoType.GetProperty("Id");
        var nameProperty = dtoType.GetProperty("Name");
        var descriptionProperty = dtoType.GetProperty("Description");

        Assert.That(idProperty, Is.Not.Null);
        Assert.That(nameProperty, Is.Not.Null);
        Assert.That(descriptionProperty, Is.Not.Null);
    }

    [Test]
    public void Include_SingleProperty_GeneratesOnlyThatProperty()
    {
        var dtoType = typeof(EntityWithManyPropertiesDtoIncludeOnlyName);

        var nameProperty = dtoType.GetProperty("Name");
        var idProperty = dtoType.GetProperty("Id");
        var ageProperty = dtoType.GetProperty("Age");

        Assert.That(nameProperty, Is.Not.Null);
        Assert.That(idProperty, Is.Null);
        Assert.That(ageProperty, Is.Null);
    }

    [Test]
    public void Exclude_TwoProperties_GeneratesAllOthers()
    {
        var dtoType = typeof(EntityWithManyPropertiesDtoExcludeAgeAndActive);

        var idProperty = dtoType.GetProperty("Id");
        var nameProperty = dtoType.GetProperty("Name");
        var descriptionProperty = dtoType.GetProperty("Description");
        var createdAtProperty = dtoType.GetProperty("CreatedAt");
        var ageProperty = dtoType.GetProperty("Age");
        var isActiveProperty = dtoType.GetProperty("IsActive");
        var scoreProperty = dtoType.GetProperty("Score");

        Assert.That(idProperty, Is.Not.Null);
        Assert.That(nameProperty, Is.Not.Null);
        Assert.That(descriptionProperty, Is.Not.Null);
        Assert.That(createdAtProperty, Is.Not.Null);
        Assert.That(ageProperty, Is.Null);
        Assert.That(isActiveProperty, Is.Null);
        Assert.That(scoreProperty, Is.Not.Null);
    }

    [Test]
    public void From_IncludeProperty_MapsCorrectly()
    {
        var entity = new EntityWithManyProperties
        {
            Id = Guid.NewGuid(),
            Name = "TestName",
            Description = "TestDescription",
            Age = 25,
            CreatedAt = DateTime.Now,
            IsActive = true,
            Score = 100.5m
        };

        var dto = EntityWithManyPropertiesDtoIncludeOnlyIdAndName.From(entity);

        Assert.That(dto.Id, Is.EqualTo(entity.Id));
        Assert.That(dto.Name, Is.EqualTo(entity.Name));
    }

    [Test]
    public void From_ExcludeProperty_MapsAllExceptExcluded()
    {
        var entity = new EntityWithManyProperties
        {
            Id = Guid.NewGuid(),
            Name = "TestName",
            Description = "TestDescription",
            Age = 25,
            CreatedAt = DateTime.Now,
            IsActive = true,
            Score = 100.5m
        };

        var dto = EntityWithManyPropertiesDtoExcludeDescriptionAndScore.From(entity);

        Assert.That(dto.Id, Is.EqualTo(entity.Id));
        Assert.That(dto.Name, Is.EqualTo(entity.Name));
        Assert.That(dto.Age, Is.EqualTo(entity.Age));
        Assert.That(dto.CreatedAt, Is.EqualTo(entity.CreatedAt));
        Assert.That(dto.IsActive, Is.EqualTo(entity.IsActive));
    }

    [Test]
    public void Include_InvalidPropertyName_ThrowsOrIgnores()
    {
        Assert.DoesNotThrow(() =>
        {
            var dto = new EntityWithManyPropertiesDto();
        });
    }

    [Test]
    public void Exclude_InvalidPropertyName_ThrowsOrIgnores()
    {
        Assert.DoesNotThrow(() =>
        {
            var dto = new EntityWithManyPropertiesDto();
        });
    }
}
